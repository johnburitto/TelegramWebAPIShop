namespace Core.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string? UserTelegramId { get; set; }

        public List<Product>? Products => new List<Product>();
    }
}
