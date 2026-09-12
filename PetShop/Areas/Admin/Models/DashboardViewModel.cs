using PetShop.DataContext.Entites;

namespace PetShop.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalBrands { get; set; }
        public int TotalBlogPosts { get; set; }

        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        
        public List<PetShop.DataContext.Entites.AdminUser> RecentUsers { get; set; } = new();
        public List<PetShop.DataContext.Entites.Order> RecentOrders { get; set; } = new();
    }
}
