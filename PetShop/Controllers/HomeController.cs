using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using PetShop.Models;
using System.Diagnostics;

namespace PetShop.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly AppDbContext _context; 

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.ProductCategories.ToListAsync();
            var animalTypes = await _context.AnimalTypes.ToListAsync();
            var products = await _context.Products.Include(p => p.Images).ToListAsync();
            var brands = await _context.Brands.ToListAsync();
            var viewModel = new HomeViewModel
            {
                Categories = categories,
                AnimalTypes = animalTypes,
                Products = products,
                Brands = brands
            };

            return View(viewModel);
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
