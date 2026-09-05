namespace PetShop.DataContext.Entites
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string? Size { get; set; }      // "S", "M", "L" (geyim üçün)
        public string? Weight { get; set; }    // "1kg", "5kg" (yem üçün)
        public string? Color { get; set; }
        public decimal PriceModifier { get; set; }
        public int StockQuantity { get; set; }
    }
}
