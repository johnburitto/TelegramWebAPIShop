namespace Core.Entities
{
    public class Discount : AuditMetadata
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float DiscoutInPercent { get; set; }
        public float NormalizedDiscount
        {
            get
            {
                return DiscoutInPercent / 100;
            }
            set
            {
                
            }
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DiscountStatus Status
        {
            get
            {
                return DateTime.UtcNow < StartDate || DateTime.Now > EndDate ? DiscountStatus.Inactive : DiscountStatus.Active;
            }
            set
            {

            }
        }

        public List<Product>? Products { get; set; }
    }

    public enum DiscountStatus
    {
        Active,
        Inactive
    }
}
