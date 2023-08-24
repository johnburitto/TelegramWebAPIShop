namespace Core.Dtos.Create
{
    public class CartCreateDto
    {
        public string? UserTelegramId { get; set; }
        public List<int> Products => new List<int>();
        public int ExpirationTimeInHours { get; set; }
    }
}
