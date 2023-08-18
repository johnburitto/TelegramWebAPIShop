namespace Core.Entities
{
    public class AuditMetadata
    {
        public string? ChangedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
    }
}
