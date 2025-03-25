using System.ComponentModel.DataAnnotations;

namespace BankAPI.Models
{
    public class AccountHolder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(20)]
        public string IdNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ResidentialAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}