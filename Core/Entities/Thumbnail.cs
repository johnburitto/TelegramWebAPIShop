namespace Core.Entities
{
    public class Thumbnail : AuditMetadata
    {
        public int Id { get; set; }
        public string? URI { get; set; }
    
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
