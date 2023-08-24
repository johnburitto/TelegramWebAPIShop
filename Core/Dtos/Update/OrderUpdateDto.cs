namespace Core.Dtos.Update
{
    public class OrderUpdateDto
    {
        public int Id { get; set; }
        public string? UserTelegramId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public List<int>? ProductsIds { get; set; }
    }
}
