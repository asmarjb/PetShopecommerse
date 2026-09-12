using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using PetShop.Models;
using PetShop.Services;
using PetShopEcommerse.Models;

namespace PetShopEcommerse.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBasketService _basketService;

        public CheckoutController(AppDbContext context, IBasketService basketService)
        {
            _context = context;
            _basketService = basketService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sessionId = _basketService.GetOrCreateSessionId();
            var basket = await _basketService.GetBasketAsync(sessionId);
            var couponCode = HttpContext.Session.GetString("AppliedCouponCode");
            await _basketService.ApplyCouponAndShippingAsync(basket, couponCode);

            var model = new CheckoutViewModel
            {
                Basket = basket,
                Country = "Azərbaycan"
            };

            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int currentUserId))
                {
                    var user = await _context.AppUsers.FindAsync(currentUserId);
                    if (user != null)
                    {
                        var nameParts = (user.FullName ?? "").Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                        model.FirstName = nameParts.Length > 0 ? nameParts[0] : "";
                        model.LastName = nameParts.Length > 1 ? nameParts[1] : "";
                        model.Email = user.Email;
                        model.Phone = user.Phone;
                        model.Address = user.Address;
                        model.City = user.City;
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            var sessionId = _basketService.GetOrCreateSessionId();
            var basket = await _basketService.GetBasketAsync(sessionId);
            var couponCode = HttpContext.Session.GetString("AppliedCouponCode");
            await _basketService.ApplyCouponAndShippingAsync(basket, couponCode);

            model.Basket = basket;
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (basket == null || basket.IsEmpty)
            {
                var emptyMsg = "Səbətiniz boşdur. Sifariş yaratmaq üçün əvvəlcə məhsul əlavə edin.";
                if (isAjax) return Json(new { success = false, message = emptyMsg });
                ModelState.AddModelError("", emptyMsg);
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName) ||
                string.IsNullOrWhiteSpace(model.Address) || string.IsNullOrWhiteSpace(model.City) ||
                string.IsNullOrWhiteSpace(model.Phone) || string.IsNullOrWhiteSpace(model.Email))
            {
                var validationMsg = "Zəhmət olmasa bütün vacib ulduzlu (*) sahələri doldurun.";
                if (isAjax) return Json(new { success = false, message = validationMsg });
                ModelState.AddModelError("", validationMsg);
                return View(model);
            }

            try
            {
                string fullAddress = $"{model.Address} {(string.IsNullOrWhiteSpace(model.Address2) ? "" : model.Address2)}, {model.City}, {(string.IsNullOrWhiteSpace(model.State) ? "" : model.State)}, {(string.IsNullOrWhiteSpace(model.Country) ? "Azərbaycan" : model.Country)}".Trim().Trim(',');

                int? orderUserId = null;
                if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
                {
                    var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdStr, out int currentUserId))
                    {
                        orderUserId = currentUserId;
                    }
                }

                var order = new Order
                {
                    SessionId = sessionId,
                    UserId = orderUserId,
                    CustomerName = $"{model.FirstName} {model.LastName}".Trim(),
                    Email = model.Email.Trim(),
                    Phone = model.Phone.Trim(),
                    ShippingAddress = fullAddress,
                    OrderNotes = model.Notes,
                    Subtotal = basket.TotalPrice,
                    DiscountAmount = basket.DiscountAmount,
                    TotalAmount = basket.FinalTotal,
                    CouponCode = basket.AppliedCouponCode,
                    PaymentMethod = model.PaymentMethod == "bank" ? "BankCard" : "CashOnDelivery",
                    PaymentStatus = model.PaymentMethod == "bank" ? "Paid" : "Pending",
                    OrderStatus = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var item in basket.Items)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        ProductVariantId = item.ProductVariantId.HasValue && item.ProductVariantId.Value > 0 ? item.ProductVariantId : null,
                        ProductName = item.ProductName + (!string.IsNullOrEmpty(item.VariantLabel) ? $" ({item.VariantLabel})" : ""),
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice
                    });
                }

                // Kupon istifadə sayını yoxla və artır
                if (!string.IsNullOrEmpty(basket.AppliedCouponCode))
                {
                    var coupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.Code.ToLower() == basket.AppliedCouponCode.ToLower());
                    if (coupon != null)
                    {
                        // Limitə çatıbsa, endirimi sil və kuponu tətbiq etmə
                        bool isLimitReached = coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value;
                        bool isExpired = coupon.ExpiryDate.HasValue && coupon.ExpiryDate.Value < DateTime.UtcNow;

                        if (isLimitReached || isExpired || !coupon.IsActive)
                        {
                            // Sifarişdən endirimi sil
                            order.DiscountAmount = 0;
                            order.TotalAmount = order.Subtotal;
                            order.CouponCode = null;
                            HttpContext.Session.Remove("AppliedCouponCode");
                        }
                        else
                        {
                            coupon.UsedCount++;
                            // Limitə çatdısa kuponu deaktiv et
                            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
                            {
                                coupon.IsActive = false;
                            }
                        }
                    }
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Səbəti və kuponu təmizlə
                await _basketService.ClearBasketAsync(sessionId);
                HttpContext.Session.Remove("AppliedCouponCode");

                if (isAjax)
                {
                    return Json(new { success = true, redirectUrl = Url.Action(nameof(Confirmation), new { id = order.Id }) });
                }

                return RedirectToAction(nameof(Confirmation), new { id = order.Id });
            }
            catch (Exception ex)
            {
                var errorMsg = "Sifariş rəsmiləşdirilərkən xəta baş verdi: " + ex.Message;
                if (isAjax) return Json(new { success = false, message = errorMsg });
                ModelState.AddModelError("", errorMsg);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }
    }
}
