using System.ComponentModel.DataAnnotations;

namespace BankAPI.Models
{
    // Model representing a withdrawal transaction
    public class Withdrawal
    {
        [Key] // Primary key
        public int Id { get; set; }
        
        [Required] // Account number is required
        [StringLength(20, MinimumLength = 5)]
        public string AccountNumber { get; set; } = string.Empty;
        
        [Required] // Navigation property to account
        public Account Account { get; set; } = null!;
        
        [Required] // Transaction date is required
        public DateTime TransactionDate { get; set; }
        
        [Required] // Amount is required
         [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")] // Must be positive
        public decimal Amount { get; set; }
    }
}