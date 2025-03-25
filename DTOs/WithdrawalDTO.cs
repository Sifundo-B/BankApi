namespace BankAPI.DTOs
{
    // DTO for withdrawal responses
    public class WithdrawalDTO
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
    }
}