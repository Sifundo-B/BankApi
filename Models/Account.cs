using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAPI.Models
{
    // Enum to represent different account types
    public enum AccountType
    {
        Cheque,     // Standard checking account
        Savings,    // Savings account
        FixedDeposit // Fixed deposit account with term
    }

    // Enum to represent account status
    public enum AccountStatus
    {
        Active,     // Account is active and can transact
        Inactive,   // Account exists but cannot transact
        Closed      // Account has been closed
    }

    // Main account model representing a bank account
    public class Account
    {
        [Key] // Primary key
        public string AccountNumber { get; set; } = string.Empty;
        
        [Required] // Account type is required
        public AccountType Type { get; set; }
        [ForeignKey("AccountHolder")]
        public int AccountHolderId { get; set; }
        public AccountHolder AccountHolder { get; set; } = null!;
        
        [Required] // Status is required
        public AccountStatus Status { get; set; }
        
        [Required] // Balance is required
        [Range(0, double.MaxValue)] // Can't have negative balance
        public decimal AvailableBalance { get; set; }
        
        // Navigation property for withdrawals (one-to-many relationship)
        public ICollection<Withdrawal> Withdrawals { get; set; } = new List<Withdrawal>();
        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public byte[] RowVersion { get; set; }
    }
}
