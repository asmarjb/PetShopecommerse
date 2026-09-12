using PetShop.DataContext.Entites;

namespace PetShop.Models
{
    public class ShopViewModel
    {
        // Products & Pagination
        public List<Product> Products { get; set; } = new List<Product>();

        // Wishlist state — product IDs already in current session's wishlist
        public HashSet<int> WishlistProductIds { get; set; } = new HashSet<int>();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalItems { get; set; }
        public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalItems / PageSize));
        public int StartItem => TotalItems == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
        public int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        // Filter options for sidebar
        public List<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        public List<AnimalType> AnimalTypes { get; set; } = new List<AnimalType>();
        public List<Brand> Brands { get; set; } = new List<Brand>();

        // Selected filter state
        public string? SearchTerm { get; set; }
        public int? SelectedCategoryId { get; set; }
        public int? SelectedAnimalTypeId { get; set; }
        public int? SelectedBrandId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
        public bool OnlyInStock { get; set; }
        public bool OnlyDiscounted { get; set; }

        public bool HasActiveFilters =>
            !string.IsNullOrWhiteSpace(SearchTerm) ||
            (SelectedCategoryId.HasValue && SelectedCategoryId.Value > 0) ||
            (SelectedAnimalTypeId.HasValue && SelectedAnimalTypeId.Value > 0) ||
            (SelectedBrandId.HasValue && SelectedBrandId.Value > 0) ||
            MinPrice.HasValue ||
            MaxPrice.HasValue ||
            OnlyInStock ||
            OnlyDiscounted ||
            (!string.IsNullOrEmpty(SortBy) && SortBy != "default");
    }
}
