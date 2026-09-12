namespace PetShop.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? Sku { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductCategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        public int? BrandId { get; set; }
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? Sku { get; set; }
        public bool IsActive { get; set; } = true;
        public int ProductCategoryId { get; set; }
        public int? BrandId { get; set; }
    }

    public class UpdateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? Sku { get; set; }
        public bool IsActive { get; set; }
        public int ProductCategoryId { get; set; }
        public int? BrandId { get; set; }
    }
}
