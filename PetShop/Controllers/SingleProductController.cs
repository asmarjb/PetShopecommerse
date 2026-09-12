using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using PetShop.Models;
using PetShop.Services;

namespace PetShopEcommerse.Controllers
{
    public class SingleProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWishlistService _wishlistService;

        public SingleProductController(AppDbContext context, IWishlistService wishlistService)
        {
            _context = context;
            _wishlistService = wishlistService;
        }

        private async Task<ClaimsPrincipal> GetCurrentPrincipalAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return User;
            }

            var adminAuth = await HttpContext.AuthenticateAsync("AdminAuth");
            if (adminAuth.Succeeded && adminAuth.Principal != null)
            {
                return adminAuth.Principal;
            }

            return User;
        }

        [HttpGet]
        [Route("product/{id:int}")]
        [Route("single-product/{id:int}")]
        [Route("SingleProduct/{id:int?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return RedirectToAction("Index", "Shop");
            }

            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Brand)
                .Include(p => p.ProductCategory)
                .Include(p => p.Variants)
                .Include(p => p.ProductAnimalTypes)
                    .ThenInclude(pat => pat.AnimalType)
                .FirstOrDefaultAsync(p => p.Id == id.Value && p.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            // Related products in same category or fallback
            var relatedProducts = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductCategory)
                .Include(p => p.Images)
                .Where(p => p.ProductCategoryId == product.ProductCategoryId && p.Id != product.Id && p.IsActive)
                .Take(4)
                .ToListAsync();

            if (relatedProducts.Count < 4)
            {
                var additional = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.ProductCategory)
                    .Include(p => p.Images)
                    .Where(p => p.Id != product.Id && !relatedProducts.Select(r => r.Id).Contains(p.Id) && p.IsActive)
                    .Take(4 - relatedProducts.Count)
                    .ToListAsync();
                relatedProducts.AddRange(additional);
            }

            // Session Wishlist state
            var sessionId = _wishlistService.GetOrCreateSessionId();
            var wishlist = await _wishlistService.GetWishlistAsync(sessionId);
            var wishlistIds = wishlist.Items.Select(i => i.ProductId).ToHashSet();

            // Reviews & Ratings
            var reviews = await _context.ProductReviews
                .Where(r => r.ProductId == product.Id && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            double averageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 1) : 5.0;
            var ratingCounts = new Dictionary<int, int> { { 5, 0 }, { 4, 0 }, { 3, 0 }, { 2, 0 }, { 1, 0 } };
            foreach (var r in reviews)
            {
                if (ratingCounts.ContainsKey(r.Rating))
                    ratingCounts[r.Rating]++;
            }

            // Check purchase verification & review status
            var principal = await GetCurrentPrincipalAsync();
            bool isUserLoggedIn = principal.Identity?.IsAuthenticated == true;
            bool hasPurchased = false;
            bool alreadyReviewed = false;
            ProductReview? userReview = null;
            string? currentUserName = null;

            if (isUserLoggedIn)
            {
                currentUserName = principal.Identity?.Name ?? "Customer";
                var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value?.Trim().ToLower();
                int currentUserId = 0;
                int.TryParse(userIdStr, out currentUserId);

                bool isAdmin = principal.IsInRole("Admin");
                hasPurchased = isAdmin || await _context.Orders
                    .Where(o => ((currentUserId > 0 && o.UserId == currentUserId) ||
                                 (!string.IsNullOrEmpty(userEmail) && o.Email.ToLower() == userEmail))
                                && o.OrderStatus != "Cancelled")
                    .AnyAsync(o => o.OrderItems.Any(oi => oi.ProductId == product.Id));

                userReview = reviews.FirstOrDefault(r =>
                    (currentUserId > 0 && r.UserId == currentUserId) ||
                    (!string.IsNullOrEmpty(userEmail) && r.CustomerEmail.ToLower() == userEmail));

                alreadyReviewed = userReview != null;
            }

            var viewModel = new SingleProductViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts,
                IsWishlisted = wishlistIds.Contains(product.Id),
                WishlistProductIds = wishlistIds,
                Reviews = reviews,
                AverageRating = averageRating,
                TotalReviews = reviews.Count,
                RatingCounts = ratingCounts,
                IsUserLoggedIn = isUserLoggedIn,
                HasPurchased = hasPurchased,
                AlreadyReviewed = alreadyReviewed,
                UserReview = userReview,
                CurrentUserName = currentUserName
            };

            return View(viewModel);
        }

        // ==========================================
        // SUBMIT OR UPDATE REVIEW (Fast rating & review)
        // ==========================================
        [HttpPost]
        [Route("product/review")]
        [Route("SingleProduct/AddReview")]
        public async Task<IActionResult> AddReview([FromBody] ReviewInputModel input)
        {
            if (input == null || input.ProductId <= 0 || input.Rating < 1 || input.Rating > 5)
            {
                return Json(new { success = false, message = "Zəhmət olmasa 1-dən 5-ə qədər ulduz seçin." });
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == input.ProductId && p.IsActive);
            if (product == null)
            {
                return Json(new { success = false, message = "Məhsul tapılmadı." });
            }

            var principal = await GetCurrentPrincipalAsync();
            bool isUserLoggedIn = principal.Identity?.IsAuthenticated == true;

            string userName = isUserLoggedIn 
                ? (principal.Identity?.Name ?? "Müştəri") 
                : (!string.IsNullOrWhiteSpace(input.CustomerName) ? input.CustomerName.Trim() : "Müştəri");

            string userEmail = isUserLoggedIn 
                ? (principal.FindFirst(ClaimTypes.Email)?.Value?.Trim().ToLower() ?? "") 
                : (!string.IsNullOrWhiteSpace(input.CustomerEmail) ? input.CustomerEmail.Trim().ToLower() : "");

            int currentUserId = 0;
            if (isUserLoggedIn)
            {
                int.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out currentUserId);
            }

            // Check if verified purchaser (or admin)
            bool isAdmin = isUserLoggedIn && principal.IsInRole("Admin");
            bool hasPurchased = isAdmin || (!string.IsNullOrEmpty(userEmail) && await _context.Orders
                .Where(o => ((currentUserId > 0 && o.UserId == currentUserId) ||
                             (!string.IsNullOrEmpty(userEmail) && o.Email.ToLower() == userEmail))
                            && o.OrderStatus != "Cancelled")
                .AnyAsync(o => o.OrderItems.Any(oi => oi.ProductId == input.ProductId)));

            string commentText = !string.IsNullOrWhiteSpace(input.Comment)
                ? input.Comment.Trim()
                : $"{input.Rating} ulduz qiymətləndirmə";

            // If user or email already reviewed, update review
            ProductReview? existingReview = null;
            if (currentUserId > 0 || !string.IsNullOrEmpty(userEmail))
            {
                existingReview = await _context.ProductReviews
                    .FirstOrDefaultAsync(r => r.ProductId == input.ProductId &&
                        ((currentUserId > 0 && r.UserId == currentUserId) ||
                         (!string.IsNullOrEmpty(userEmail) && r.CustomerEmail.ToLower() == userEmail)));
            }

            if (existingReview != null)
            {
                existingReview.Rating = input.Rating;
                existingReview.Comment = commentText;
                existingReview.CustomerName = userName;
                if (hasPurchased) existingReview.IsVerifiedPurchase = true;
                existingReview.UpdatedAt = DateTime.UtcNow;
                _context.ProductReviews.Update(existingReview);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Ulduz və rəyiniz uğurla yeniləndi!", isUpdate = true });
            }
            else
            {
                var review = new ProductReview
                {
                    ProductId = input.ProductId,
                    UserId = currentUserId > 0 ? currentUserId : null,
                    CustomerName = userName,
                    CustomerEmail = !string.IsNullOrEmpty(userEmail) ? userEmail : "guest@petshop.az",
                    Rating = input.Rating,
                    Comment = commentText,
                    IsVerifiedPurchase = hasPurchased,
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ProductReviews.Add(review);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Təşəkkür edirik! Ulduz və rəyiniz qeydə alındı.", isUpdate = false });
            }
        }
    }
}
