namespace Core.Dtos.Create
{
    public class DiscountCreateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float DiscoutInPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int>? ProductsIds { get; set; }
    }
}
