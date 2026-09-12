using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PetShop.DataContext.Entites
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;        // Məs: "Yem", "Geyim", "Oyuncaq"
        public string Slug { get; set; } = string.Empty; // Məs: "food", "cloth"

        public string? IconClass { get; set; }
        public string? ImageUrl { get; set; }
        public int? ParentCategoryId { get; set; }

        [ValidateNever]
        public ProductCategory? ParentCategory { get; set; }

        [ValidateNever]
        public ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();

        [ValidateNever]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
