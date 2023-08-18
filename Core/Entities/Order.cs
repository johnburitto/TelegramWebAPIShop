namespace Core.Entities
{
    public class Order: AuditMetadata
    {
        public int Id { get; set; }
        public string? UserTelegramId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        
        public List<Product>? Products { get; set; }
    }

    public enum OrderStatus
    {
        Created,
        Approved,
        Paid,
        Send
    }
}
