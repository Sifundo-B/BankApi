using BankAPI.Models;
using BankAPI.Models.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Withdrawal> Withdrawals { get; set; }
        public DbSet<AccountHolder> AccountHolders { get; set; }

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