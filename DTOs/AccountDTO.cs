namespace BankAPI.DTOs
{
    // Data Transfer Object for Account - used for API responses
    public class AccountDTO
    {
         /// <summary>
    /// The account number
    /// </summary>
    /// <example>1000000001</example>
        public string AccountNumber { get; set; } = string.Empty;
         /// <summary>
    /// The type of account
    /// </summary>
    /// <example>Savings</example>
        public string Type { get; set; } = string.Empty; // String representation of enum
        /// <summary>
    /// Current status of the account
    /// </summary>
    /// <example>Active</example>
        public string Status { get; set; } = string.Empty; // String representation of enum
        /// <summary>
    /// Available balance in the account
    /// </summary>
    /// <example>15000.00</example>
        public decimal AvailableBalance { get; set; }
        /// <summary>
    /// Account holder information
    /// </summary>
        public AccountHolderDTO AccountHolder { get; set; } = null!;
    }
}
