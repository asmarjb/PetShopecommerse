namespace PetShop.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? IconClass { get; set; }
        public int? ParentCategoryId { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? IconClass { get; set; }
        public int? ParentCategoryId { get; set; }
    }

    public class UpdateCategoryDto
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? IconClass { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
