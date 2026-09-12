using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class CouponsController : Controller
    {
        private readonly AppDbContext _context;

        public CouponsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Coupons.OrderByDescending(c => c.CreatedAt).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                coupon.CreatedAt = DateTime.UtcNow;
                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupon coupon)
        {
            if (id != coupon.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCoupon = await _context.Coupons.FindAsync(id);
                    if (existingCoupon == null) return NotFound();

                    // Yalnız admin tərəfindən redaktə edilən sahələri yenilə
                    // UsedCount və CreatedAt toxunulmaz qalır
                    existingCoupon.Code = coupon.Code;
                    existingCoupon.DiscountType = coupon.DiscountType;
                    existingCoupon.DiscountValue = coupon.DiscountValue;
                    existingCoupon.MinimumOrderAmount = coupon.MinimumOrderAmount;
                    existingCoupon.MaxDiscountAmount = coupon.MaxDiscountAmount;
                    existingCoupon.UsageLimit = coupon.UsageLimit;
                    existingCoupon.ExpiryDate = coupon.ExpiryDate;
                    existingCoupon.IsActive = coupon.IsActive;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponExists(coupon.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.Id == id);
        }
    }
}

