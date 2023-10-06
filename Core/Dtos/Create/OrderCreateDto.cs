using Core.Entities;

namespace Core.Dtos.Create
{
    public class OrderCreateDto
    {
        public OrderStatus Status { get; set; }
        public string? UserTelegramId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public float? TotalPrise { get; set; }
        public List<int>? ProductsIds { get; set; }
    }
}
