namespace Core.Dtos.Create
{
    public class OrdeCreateDto
    {
        public string? UserTelegramId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public List<int>? ProductIds { get; set; }
    }
}
