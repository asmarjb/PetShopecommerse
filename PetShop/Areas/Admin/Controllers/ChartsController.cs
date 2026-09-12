using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.Areas.Admin.Models;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class ChartsController : Controller
    {
        private readonly AppDbContext _context;

        public ChartsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Gather statistics for charts
            var productsByCategory = await _context.Products
                .Include(p => p.ProductCategory)
                .GroupBy(p => p.ProductCategory.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync();

            var activeVsInactiveProducts = await _context.Products
                .GroupBy(p => p.IsActive)
                .Select(g => new { IsActive = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.CategoriesLabels = productsByCategory.Select(x => x.Category).ToList();
            ViewBag.CategoriesData = productsByCategory.Select(x => x.Count).ToList();

            ViewBag.ActiveProductsCount = activeVsInactiveProducts.FirstOrDefault(x => x.IsActive)?.Count ?? 0;
            ViewBag.InactiveProductsCount = activeVsInactiveProducts.FirstOrDefault(x => !x.IsActive)?.Count ?? 0;

            return View();
        }
    }
}

