using System.ComponentModel.DataAnnotations;

namespace BankAPI.DTOs
{
    // DTO for creating a new withdrawal
    public class CreateWithdrawalDTO
    {
         /// <summary>
    /// The account number to withdraw from
    /// </summary>
    /// <example>1000000001</example>
        [Required(ErrorMessage = "Account number is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Account number must be between 5 and 20 characters")]
        public string AccountNumber { get; set; } = string.Empty;
        /// <summary>
    /// The amount to withdraw (must be positive)
    /// </summary>
    /// <example>100.00</example>
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}
