namespace Core.Entities
{
    public class Product : AuditMetadata
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }

        public List<Order>? Orders { get; set; }
        public List<Discount>? Discounts { get; set; }
        public List<Thumbnail>? Thumbnails { get; set; }
    }
}
