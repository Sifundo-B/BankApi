using System.Text.Json;
using BankAPI.Models.Audit;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BankAPI.Models.Helper
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string TableName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public Dictionary<string, object> KeyValues { get; } = new();
        public Dictionary<string, object> OldValues { get; } = new();
        public Dictionary<string, object> NewValues { get; } = new();
        public AuditType AuditType { get; set; }
        public List<PropertyEntry> TemporaryProperties { get; } = new();

        public bool HasTemporaryProperties => TemporaryProperties.Any();

        public AuditLog ToAudit()
        {
            return new AuditLog
            {
                TableName = TableName,
                AuditType = AuditType,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = UserId,
                RecordId = string.Join(",", KeyValues.Select(kv => $"{kv.Key}={kv.Value}")),
                OldValues = OldValues.Count == 0 ? string.Empty : JsonSerializer.Serialize(OldValues),
                NewValues = NewValues.Count == 0 ? string.Empty : JsonSerializer.Serialize(NewValues)
            };
        }

    }
}