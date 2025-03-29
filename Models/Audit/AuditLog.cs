using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAPI.Models.Audit
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TableName { get; set; } = string.Empty;

        [Required]
        public AuditType AuditType { get; set; }

        [Required]
        public string RecordId { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string OldValues { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string NewValues { get; set; } = string.Empty;

        [Required]
        public string ChangedBy { get; set; } = string.Empty;

        [Required]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public string? IpAddress { get; set; }
    }
}