namespace PetShop.DataContext.Entites
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;        // Məs: "Yem", "Geyim", "Oyuncaq"
        public string Slug { get; set; } = null!;         // Məs: "food", "cloth"

        public string? IconClass { get; set; }
        public int? ParentCategoryId { get; set; }
        public ProductCategory? ParentCategory { get; set; }
        public ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
