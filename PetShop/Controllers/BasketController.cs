using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.Services;
using PetShopEcommerse.Models;

namespace PetShopEcommerse.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly AppDbContext _context;

        public BasketController(IBasketService basketService, AppDbContext context)
        {
            _basketService = basketService;
            _context = context;
        }

        private string? GetSessionCouponCode()
        {
            return HttpContext.Session.GetString("AppliedCouponCode");
        }

        // GET: /Basket
        // GET: /Basket/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sessionId = _basketService.GetOrCreateSessionId();
            var basket = await _basketService.GetBasketAsync(sessionId);
            await _basketService.ApplyCouponAndShippingAsync(basket, GetSessionCouponCode());
            return View(basket);
        }

        // GET: /Basket/GetBasket
        [HttpGet]
        public async Task<IActionResult> GetBasket()
        {
            var sessionId = _basketService.GetOrCreateSessionId();
            var basket = await _basketService.GetBasketAsync(sessionId);
            await _basketService.ApplyCouponAndShippingAsync(basket, GetSessionCouponCode());

            return Ok(new
            {
                success = true,
                items = basket.Items,
                totalPrice = basket.TotalPrice,
                totalItemCount = basket.TotalItemCount,
                isEmpty = basket.IsEmpty,
                appliedCouponCode = basket.AppliedCouponCode,
                discountAmount = basket.DiscountAmount,
                discountPercent = basket.DiscountPercent,
                shippingFee = basket.ShippingFee,
                isFreeShipping = basket.IsFreeShipping,
                amountNeededForFreeShipping = basket.AmountNeededForFreeShipping,
                freeShippingProgress = basket.FreeShippingProgress,
                finalTotal = basket.FinalTotal,
                data = basket
            });
        }

        // POST: /Basket/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            if (dto == null || dto.ProductId <= 0)
            {
                return BadRequest(new { success = false, message = "Məhsul məlumatları düzgün təqdim edilməyib." });
            }

            if (dto.Quantity <= 0) dto.Quantity = 1;

            var sessionId = _basketService.GetOrCreateSessionId();
            var result = await _basketService.AddToCartAsync(sessionId, dto.ProductId, dto.ProductVariantId, dto.Quantity);

            if (!result.Success)
            {
                if (result.Message.Contains("tapılmadı"))
                {
                    return NotFound(new { success = false, message = result.Message });
                }
                return BadRequest(new { success = false, message = result.Message });
            }

            var cart = result.Cart ?? await _basketService.GetBasketAsync(sessionId);
            await _basketService.ApplyCouponAndShippingAsync(cart, GetSessionCouponCode());

            return Ok(new
            {
                success = true,
                message = result.Message,
                productId = dto.ProductId,
                productVariantId = dto.ProductVariantId,
                remainingStock = result.RemainingStock,
                items = cart.Items,
                totalPrice = cart.TotalPrice,
                totalItemCount = cart.TotalItemCount,
                isEmpty = cart.IsEmpty,
                appliedCouponCode = cart.AppliedCouponCode,
                discountAmount = cart.DiscountAmount,
                discountPercent = cart.DiscountPercent,
                shippingFee = cart.ShippingFee,
                isFreeShipping = cart.IsFreeShipping,
                amountNeededForFreeShipping = cart.AmountNeededForFreeShipping,
                freeShippingProgress = cart.FreeShippingProgress,
                finalTotal = cart.FinalTotal,
                data = cart
            });
        }

        // POST: /Basket/AddToCart/{productId}
        [HttpPost]
        [Route("Basket/AddToCart/{productId:int}")]
        public async Task<IActionResult> AddToCartById(int productId, [FromQuery] int? variantId = null, [FromQuery] int quantity = 1)
        {
            return await AddToCart(new AddToCartDto
            {
                ProductId = productId,
                ProductVariantId = variantId,
                Quantity = quantity > 0 ? quantity : 1
            });
        }

        // POST: /Basket/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityDto dto)
        {
            if (dto == null || dto.CartItemId <= 0)
            {
                return BadRequest(new { success = false, message = "Səbət elementi düzgün seçilməyib." });
            }

            var sessionId = _basketService.GetOrCreateSessionId();
            var result = await _basketService.UpdateQuantityAsync(sessionId, dto.CartItemId, dto.Quantity);

            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message });
            }

            var cart = result.Cart ?? await _basketService.GetBasketAsync(sessionId);
            await _basketService.ApplyCouponAndShippingAsync(cart, GetSessionCouponCode());

            return Ok(new
            {
                success = true,
                message = result.Message,
                items = cart.Items,
                totalPrice = cart.TotalPrice,
                totalItemCount = cart.TotalItemCount,
                isEmpty = cart.IsEmpty,
                appliedCouponCode = cart.AppliedCouponCode,
                discountAmount = cart.DiscountAmount,
                discountPercent = cart.DiscountPercent,
                shippingFee = cart.ShippingFee,
                isFreeShipping = cart.IsFreeShipping,
                amountNeededForFreeShipping = cart.AmountNeededForFreeShipping,
                freeShippingProgress = cart.FreeShippingProgress,
                finalTotal = cart.FinalTotal,
                data = cart
            });
        }

        // POST: /Basket/RemoveFromCart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartDto dto)
        {
            if (dto == null || (!dto.CartItemId.HasValue && !dto.ProductId.HasValue))
            {
                return BadRequest(new { success = false, message = "Silinəcək məhsul təyin edilməyib." });
            }

            var sessionId = _basketService.GetOrCreateSessionId();
            var result = await _basketService.RemoveFromCartAsync(sessionId, dto.CartItemId, dto.ProductId, dto.ProductVariantId);

            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message });
            }

            var cart = result.Cart ?? await _basketService.GetBasketAsync(sessionId);
            await _basketService.ApplyCouponAndShippingAsync(cart, GetSessionCouponCode());

            return Ok(new
            {
                success = true,
                message = result.Message,
                productId = result.RemovedProductId,
                remainingStock = result.RestoredStock,
                items = cart.Items,
                totalPrice = cart.TotalPrice,
                totalItemCount = cart.TotalItemCount,
                isEmpty = cart.IsEmpty,
                appliedCouponCode = cart.AppliedCouponCode,
                discountAmount = cart.DiscountAmount,
                discountPercent = cart.DiscountPercent,
                shippingFee = cart.ShippingFee,
                isFreeShipping = cart.IsFreeShipping,
                amountNeededForFreeShipping = cart.AmountNeededForFreeShipping,
                freeShippingProgress = cart.FreeShippingProgress,
                finalTotal = cart.FinalTotal,
                data = cart
            });
        }

        // POST: /Basket/ApplyCoupon
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Code))
            {
                return BadRequest(new { success = false, message = "Zəhmət olmasa kupon kodunu daxil edin." });
            }

            var cleanCode = dto.Code.Trim();
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code.ToLower() == cleanCode.ToLower() && c.IsActive);

            if (coupon == null)
            {
                return BadRequest(new { success = false, message = "Daxil edilən kupon kodu mövcud deyil və ya aktiv deyil." });
            }

            if (coupon.ExpiryDate.HasValue && coupon.ExpiryDate.Value < DateTime.UtcNow)
            {
                return BadRequest(new { success = false, message = "Bu kuponun istifadə müddəti bitmişdir." });
            }

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
            {
                return BadRequest(new { success = false, message = "Bu kuponun istifadə limiti dolmuşdur." });
            }

            var sessionId = _basketService.GetOrCreateSessionId();
            var basket = await _basketService.GetBasketAsync(sessionId);

            if (basket.IsEmpty)
            {
                return BadRequest(new { success = false, message = "Kupon tətbiq etmək üçün səbətinizdə məhsul olmalıdır." });
            }

            if (coupon.MinimumOrderAmount.HasValue && basket.TotalPrice < coupon.MinimumOrderAmount.Value)
            {
                return BadRequest(new { success = false, message = $"Bu kupon minimum {coupon.MinimumOrderAmount.Value:0.00} ₼ sifarişlər üçün keçərlidir." });
            }

            HttpContext.Session.SetString("AppliedCouponCode", coupon.Code);
            await _basketService.ApplyCouponAndShippingAsync(basket, coupon.Code);

            return Ok(new
            {
                success = true,
                message = $"'{coupon.Code}' kuponu uğurla tətbiq edildi!",
                appliedCouponCode = basket.AppliedCouponCode,
                discountAmount = basket.DiscountAmount,
                discountPercent = basket.DiscountPercent,
                shippingFee = basket.ShippingFee,
                isFreeShipping = basket.IsFreeShipping,
                amountNeededForFreeShipping = basket.AmountNeededForFreeShipping,
                freeShippingProgress = basket.FreeShippingProgress,
                finalTotal = basket.FinalTotal,
                totalPrice = basket.TotalPrice,
                totalItemCount = basket.TotalItemCount,
                data = basket
            });
        }

        // POST: /Basket/RemoveCoupon
        [HttpPost]
        public async Task<IActionResult> RemoveCoupon()
        {
            HttpContext.Session.Remove("AppliedCouponCode");
            var sessionId = _basketService.GetOrCreateSessionId();
            var basket = await _basketService.GetBasketAsync(sessionId);
            await _basketService.ApplyCouponAndShippingAsync(basket, null);

            return Ok(new
            {
                success = true,
                message = "Kupon ləğv edildi.",
                appliedCouponCode = (string?)null,
                discountAmount = 0m,
                discountPercent = (decimal?)null,
                shippingFee = basket.ShippingFee,
                isFreeShipping = basket.IsFreeShipping,
                amountNeededForFreeShipping = basket.AmountNeededForFreeShipping,
                freeShippingProgress = basket.FreeShippingProgress,
                finalTotal = basket.FinalTotal,
                totalPrice = basket.TotalPrice,
                totalItemCount = basket.TotalItemCount,
                data = basket
            });
        }
    }
}
