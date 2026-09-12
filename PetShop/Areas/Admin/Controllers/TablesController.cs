using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class TablesController : Controller
    {
        private readonly AppDbContext _context;

        public TablesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // We use the Tables page to display Products as an example of a data table
            var products = await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Brand)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(products);
        }
    }
}

