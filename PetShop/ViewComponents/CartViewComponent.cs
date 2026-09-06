using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShopEcommerse.Models;
using System.Text.Json;

namespace PetShopEcommerse.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;

        public CartViewComponent(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var json = HttpContext.Session.GetString("Basket");
            var sessionItems = string.IsNullOrEmpty(json)
                ? new List<BasketItemViewModel>()
                : JsonSerializer.Deserialize<List<BasketItemViewModel>>(json)!;

            var viewModel = new BasketViewModel();
            var productIds = sessionItems.Select(i => i.ProductId).ToList();

            var products = await _dbContext.Products
                .Include(p => p.Images)
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            foreach (var item in sessionItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null) continue;

                viewModel.Items.Add(new BasketItemDetailViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.DiscountPrice ?? product.Price,
                    Quantity = item.Quantity,
                    ProductImagePath = product.Images.FirstOrDefault(i => i.IsMain)?.ImageUrl ?? "no-image.png"
                });
            }

            return View(viewModel);
        }
    }
}
