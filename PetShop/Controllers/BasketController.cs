using Microsoft.AspNetCore.Mvc;
using PetShop.DataContext;
using PetShopEcommerse.Models;

namespace PetShopEcommerse.Controllers
{
    public class BasketController : Controller
    {
      
        private readonly AppDbContext _context;


        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
       
    }
}
