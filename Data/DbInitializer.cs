using BankAPI.Models;
using BankAPI.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Apply any pending migrations
            await context.Database.MigrateAsync();

            // Seed roles if they don't exist
            string[] roleNames = { Role.Admin, Role.Banker, Role.Customer, Role.Auditor };
            
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Check if admin user already exists
            if (await userManager.FindByEmailAsync("admin@bank.com") == null)
            {
                // Create admin user
                var adminUser = new User
                {
                    UserName = "admin@bank.com",
                    Email = "admin@bank.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var createAdmin = await userManager.CreateAsync(adminUser, "Admin@123");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Role.Admin);
                }
            }

            // Check if banker user exists
            if (await userManager.FindByEmailAsync("banker@bank.com") == null)
            {
                var bankerUser = new User
                {
                    UserName = "banker@bank.com",
                    Email = "banker@bank.com",
                    FirstName = "Banker",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var createBanker = await userManager.CreateAsync(bankerUser, "Banker@123");
                if (createBanker.Succeeded)
                {
                    await userManager.AddToRoleAsync(bankerUser, Role.Banker);
                }
            }

            // Seed account holders if they don't exist
            if (!context.AccountHolders.Any())
            {
                var johnDoe = new User
                {
                    UserName = "john.doe@example.com",
                    Email = "john.doe@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(johnDoe, "Customer@123");
                await userManager.AddToRoleAsync(johnDoe, Role.Customer);

                var janeSmith = new User
                {
                    UserName = "jane.smith@example.com",
                    Email = "jane.smith@example.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(janeSmith, "Customer@123");
                await userManager.AddToRoleAsync(janeSmith, Role.Customer);

                // Add account holders with their accounts
                var accountHolders = new AccountHolder[]
                {
                    new AccountHolder
                    {
                        UserId = johnDoe.Id,
                        FirstName = "John",
                        LastName = "Doe",
                        DateOfBirth = new DateTime(1985, 5, 15),
                        IdNumber = "8505151234088",
                        ResidentialAddress = "123 Main Street, Johannesburg, 2000",
                        MobileNumber = "0825551234",
                        Email = "john.doe@example.com",
                        Accounts = new List<Account>
                        {
                            new Account
                            {
                                AccountNumber = "1000000001",
                                Type = AccountType.Savings,
                                Status = AccountStatus.Active,
                                AvailableBalance = 15000.00m
                            },
                            new Account
                            {
                                AccountNumber = "1000000002",
                                Type = AccountType.Cheque,
                                Status = AccountStatus.Active,
                                AvailableBalance = 5000.00m
                            }
                        }
                    },
                    new AccountHolder
                    {
                        UserId = janeSmith.Id,
                        FirstName = "Jane",
                        LastName = "Smith",
                        DateOfBirth = new DateTime(1990, 8, 22),
                        IdNumber = "9008220987654",
                        ResidentialAddress = "456 Oak Avenue, Cape Town, 8000",
                        MobileNumber = "0835555678",
                        Email = "jane.smith@example.com",
                        Accounts = new List<Account>
                        {
                            new Account
                            {
                                AccountNumber = "1000000003",
                                Type = AccountType.Savings,
                                Status = AccountStatus.Active,
                                AvailableBalance = 25000.00m
                            },
                            new Account
                            {
                                AccountNumber = "1000000004",
                                Type = AccountType.FixedDeposit,
                                Status = AccountStatus.Active,
                                AvailableBalance = 100000.00m
                            }
                        }
                    }
                };

                context.AccountHolders.AddRange(accountHolders);
                await context.SaveChangesAsync();
            }
        }
    }
}