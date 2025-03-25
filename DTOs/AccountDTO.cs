namespace BankAPI.DTOs
{
    // Data Transfer Object for Account - used for API responses
    public class AccountDTO
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // String representation of enum
        public string Status { get; set; } = string.Empty; // String representation of enum
        public decimal AvailableBalance { get; set; }
        public AccountHolderDTO AccountHolder { get; set; } = null!;
    }
}
