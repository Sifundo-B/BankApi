public class AuditLog
{
    public int Id { get; set; }
    public string Action { get; set; } // "Create", "Update", "Delete"
    public string TableName { get; set; }
    public string RecordId { get; set; }
    public string ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}