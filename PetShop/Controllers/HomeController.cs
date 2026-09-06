using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using PetShop.Models;
using PetShopEcommerse.Models;
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

        CookieOptions cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(7),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
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


        [HttpPost]
        public async Task<IActionResult> AddBasket(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return BadRequest();
            }
            var basket = Request.Cookies["basket"];
            List<BasketItemViewModel> basketItemViewModel;
            if (Request.Cookies.TryGetValue("basket", out var basketCookie))
            {
                basketItemViewModel = System.Text.Json.JsonSerializer.Deserialize<List<BasketItemViewModel>>(basketCookie) ?? new List<BasketItemViewModel>();
            }
            else
            {
                basketItemViewModel = new List<BasketItemViewModel>();
            }
            Response.Cookies.Append("basket", System.Text.Json.JsonSerializer.Serialize(basketItemViewModel));

            var existProduct = basketItemViewModel.FirstOrDefault(x => x.ProductId == product.Id);
            if (existProduct != null) { existProduct.Quantity++; }
            else
            {
                basketItemViewModel.Add(new BasketItemViewModel
                {
                    ProductId = product.Id,
                    Quantity = 1
                });
            }

            Response.Cookies.Append("basket", System.Text.Json.JsonSerializer.Serialize(basketItemViewModel));


            BasketViewModel basketViewModel = new BasketViewModel();
            List<BasketItemDetailViewModel> basketItemsDetail = new List<BasketItemDetailViewModel>();
            foreach (var item in basketItemViewModel)
            {
                product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    basketItemsDetail.Add(new BasketItemDetailViewModel
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = item.Quantity,
                        ProductImagePath = product.Images.FirstOrDefault(i => i.IsMain)?.ImageUrl ?? "/images/default-product.jpg",
                    });
                }

            }
            basketViewModel.Items = basketItemsDetail;
            return Json(basketViewModel);
        }
    }
}
