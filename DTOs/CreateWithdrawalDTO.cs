using System.ComponentModel.DataAnnotations;

namespace BankAPI.DTOs
{
    // DTO for creating a new withdrawal
    public class CreateWithdrawalDTO
    {
        [Required(ErrorMessage = "Account number is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Account number must be between 5 and 20 characters")]
        public string AccountNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}
