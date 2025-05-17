namespace Demirbaslar.API.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string ActionType { get; set; } // "CREATE", "UPDATE", "DELETE"
        public int RecordId { get; set; }
        public string UserId { get; set; } // Важно: string для Identity
        public AppUsers User { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
        public string ChangesJson { get; set; }
    }
}
