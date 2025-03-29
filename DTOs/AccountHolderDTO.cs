namespace BankAPI.DTOs
{
    public class AccountHolderDTO
    {
        /// <summary>
        /// Holder's first name
        /// </summary>
        /// <example>John</example>
        public string FirstName { get; set; } = string.Empty;
        /// <summary>
        /// Holder's last name
        /// </summary>
        /// <example>Doe</example>
        public string LastName { get; set; } = string.Empty;
        /// <summary>
        /// National identification number
        /// </summary>
        /// <example>8505151234088</example>
        public string IdNumber { get; set; } = string.Empty;
        /// <summary>
        /// Contact email address
        /// </summary>
        /// <example>john.doe@example.com</example>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Mobile phone number
        /// </summary>
        /// <example>0825551234</example>
        public string MobileNumber { get; set; } = string.Empty;
    }
}
