using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Apply any pending migrations
            context.Database.Migrate();

            // Check if data already exists
            if (context.AccountHolders.Any())
            {
                return; // DB has been seeded
            }

            // Add account holders
            var accountHolders = new AccountHolder[]
            {
                new AccountHolder
                {
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
            context.SaveChanges();
        }
    }
}