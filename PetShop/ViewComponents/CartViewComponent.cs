using Microsoft.AspNetCore.Mvc;
using PetShop.Services;
using PetShopEcommerse.Models;

namespace PetShopEcommerse.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly IBasketService _basketService;

        public CartViewComponent(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sessionId = _basketService.GetOrCreateSessionId();
            var viewModel = await _basketService.GetBasketAsync(sessionId);
            var couponCode = HttpContext.Session.GetString("AppliedCouponCode");
            await _basketService.ApplyCouponAndShippingAsync(viewModel, couponCode);
            return View(viewModel);
        }
    }
}
