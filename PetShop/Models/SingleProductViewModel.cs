using PetShop.DataContext.Entites;

namespace PetShop.Models
{
    public class SingleProductViewModel
    {
        public Product Product { get; set; } = null!;
        public List<Product> RelatedProducts { get; set; } = new();
        public bool IsWishlisted { get; set; }
        public HashSet<int> WishlistProductIds { get; set; } = new();

        // Reviews & Ratings
        public List<ProductReview> Reviews { get; set; } = new();
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingCounts { get; set; } = new()
        {
            { 5, 0 }, { 4, 0 }, { 3, 0 }, { 2, 0 }, { 1, 0 }
        };

        // Purchase & Review Permissions
        public bool IsUserLoggedIn { get; set; }
        public bool HasPurchased { get; set; }
        public bool AlreadyReviewed { get; set; }
        public ProductReview? UserReview { get; set; }
        public string? CurrentUserName { get; set; }
    }

    public class ReviewInputModel
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
    }
}
