using PetShop.DataContext.Entites;

namespace PetShop.Models
{
    public class HomeViewModel
    {
        public List<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        public List<AnimalType> AnimalTypes { get; set; } = new List<AnimalType>();
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Brand> Brands { get; set; } = new List<Brand>();

        // Wishlisted product IDs for current session
        public HashSet<int> WishlistProductIds { get; set; } = new HashSet<int>();
    }
}
