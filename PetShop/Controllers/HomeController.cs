using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using PetShop.Models;
using PetShop.Services;
using PetShopEcommerse.Models;
using System.Diagnostics;

namespace PetShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWishlistService _wishlistService;

        public HomeController(AppDbContext context, IWishlistService wishlistService)
        {
            _context = context;
            _wishlistService = wishlistService;
        }

        public async Task<IActionResult> Index()
        {
            var hmw = new HomeViewModel
            {
                AnimalTypes = await _context.AnimalTypes.ToListAsync(),
                Categories = await _context.ProductCategories.ToListAsync(),
                Brands = await _context.Brands.ToListAsync(),
                Products = await _context.Products
                           .Include(p => p.Brand)
                           .Include(p => p.ProductCategory)
                           .Include(p => p.Images)
                           .Include(p => p.ProductAnimalTypes)
                           .ThenInclude(pat => pat.AnimalType)
                           .ToListAsync()
            };

            // Load wishlisted product IDs for current session
            var sessionId = _wishlistService.GetOrCreateSessionId();
            var wishlist = await _wishlistService.GetWishlistAsync(sessionId);
            hmw.WishlistProductIds = wishlist.Items.Select(i => i.ProductId).ToHashSet();

            return View(hmw);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
