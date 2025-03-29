using BankAPI.Models;
using BankAPI.Models.Auth;
using BankAPI.Models.Audit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BankAPI.Models.Helper;

namespace BankAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Withdrawal> Withdrawals { get; set; }
        public DbSet<AccountHolder> AccountHolders { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Override SaveChanges to implement audit logging
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name,
                    UserId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System"
                };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            // Save audit entities that have all the modifications
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }

            // Keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                // Get the final value of the temporary properties
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                // Save the Audit entry
                AuditLogs.Add(auditEntry.ToAudit());
            }

            return SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This line is CRUCIAL for Identity to work properly
            base.OnModelCreating(modelBuilder);

            // Configure AccountHolder
            modelBuilder.Entity<AccountHolder>(entity =>
            {
                entity.Property(ah => ah.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(ah => ah.LastName).IsRequired().HasMaxLength(50);
                entity.Property(ah => ah.IdNumber).IsRequired().HasMaxLength(20);
                entity.Property(ah => ah.ResidentialAddress).IsRequired().HasMaxLength(200);
                entity.Property(ah => ah.MobileNumber).IsRequired().HasMaxLength(20);
                entity.Property(ah => ah.Email).IsRequired().HasMaxLength(100);
            });

            // Configure Account
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.AccountNumber);
                entity.Property(a => a.AccountNumber).HasMaxLength(20).IsRequired();
                entity.Property(a => a.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(a => a.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(a => a.AvailableBalance).HasColumnType("decimal(18,2)").IsRequired();

                entity.HasOne(a => a.AccountHolder)
                    .WithMany(ah => ah.Accounts)
                    .HasForeignKey(a => a.AccountHolderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Withdrawal
            modelBuilder.Entity<Withdrawal>(entity =>
            {
                entity.Property(w => w.AccountNumber).HasMaxLength(20).IsRequired();
                entity.Property(w => w.Amount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(w => w.TransactionDate).IsRequired();

                entity.HasOne(w => w.Account)
                    .WithMany(a => a.Withdrawals)
                    .HasForeignKey(w => w.AccountNumber)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

}