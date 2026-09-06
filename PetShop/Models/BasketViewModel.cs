namespace PetShopEcommerse.Models
{
 
    public class BasketItemViewModel
    {
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }


    public class BasketItemDetailViewModel
    {
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? VariantLabel { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ProductImagePath { get; set; } = null!;
        public int AvailableStock { get; set; }

        public decimal TotalPrice => UnitPrice * Quantity;
    }

    public class BasketViewModel
    {
        public List<BasketItemDetailViewModel> Items { get; set; } = new();

        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
        public int TotalItemCount => Items.Sum(i => i.Quantity);
    }
}
