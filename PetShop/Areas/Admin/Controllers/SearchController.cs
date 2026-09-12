using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new SearchViewModel());
            }

            query = query.Trim();

            // Search Products
            var products = await _context.Products
                .Include(p => p.ProductCategory)
                .Where(p => p.Name.Contains(query) || (p.Sku != null && p.Sku.Contains(query)))
                .Take(10)
                .ToListAsync();

            // Search Orders
            int.TryParse(query, out int orderId);
            var orders = await _context.Orders
                .Where(o => o.Id == orderId || o.CustomerName.Contains(query) || o.Email.Contains(query))
                .Take(10)
                .ToListAsync();

            // Search Users
            var users = await _context.AdminUsers
                .Where(u => u.FullName.Contains(query) || u.Email.Contains(query))
                .Take(10)
                .ToListAsync();

            var model = new SearchViewModel
            {
                Query = query,
                Products = products,
                Orders = orders,
                Users = users
            };

            return View(model);
        }
    }

    public class SearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public List<AdminUser> Users { get; set; } = new();
    }
}

