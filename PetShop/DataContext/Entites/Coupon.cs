namespace PetShop.DataContext.Entites
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        
        public string DiscountType { get; set; } = "Percentage"; // "Percentage" or "FixedAmount"
        public decimal DiscountValue { get; set; }
        
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
