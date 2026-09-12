using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using PetShopEcommerse.Models;

namespace PetShop.Services
{
    public class WishlistService : IWishlistService
    {
        private const string SessionCookieName = "petshop_cart_session";
        private const string SessionKey = "CartSessionId";

        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WishlistService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetOrCreateSessionId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return Guid.NewGuid().ToString("N");
            }

            if (httpContext.Request.Cookies.TryGetValue(SessionCookieName, out var cookieSessionId) &&
                !string.IsNullOrWhiteSpace(cookieSessionId))
            {
                httpContext.Session.SetString(SessionKey, cookieSessionId);
                return cookieSessionId;
            }

            var sessionSessionId = httpContext.Session.GetString(SessionKey);
            if (!string.IsNullOrWhiteSpace(sessionSessionId))
            {
                AppendSessionCookie(httpContext, sessionSessionId);
                return sessionSessionId;
            }

            var newSessionId = Guid.NewGuid().ToString("N");
            httpContext.Session.SetString(SessionKey, newSessionId);
            AppendSessionCookie(httpContext, newSessionId);

            return newSessionId;
        }

        private static void AppendSessionCookie(HttpContext httpContext, string sessionId)
        {
            if (httpContext.Response.HasStarted) return;

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = httpContext.Request.IsHttps
            };

            httpContext.Response.Cookies.Append(SessionCookieName, sessionId, cookieOptions);
        }

        public async Task<WishlistViewModel> GetWishlistAsync(string sessionId)
        {
            var viewModel = new WishlistViewModel();
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return viewModel;
            }

            var wishlist = await _context.Wishlists
                .AsNoTracking()
                .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Product)
                        .ThenInclude(p => p.Images)
                .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Product)
                        .ThenInclude(p => p.ProductCategory)
                .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.ProductVariant)
                .FirstOrDefaultAsync(w => w.SessionId == sessionId);

            if (wishlist == null || !wishlist.WishlistItems.Any())
            {
                return viewModel;
            }

            foreach (var item in wishlist.WishlistItems.OrderByDescending(wi => wi.AddedAt))
            {
                if (item.Product == null) continue;

                var imageFileName = item.Product.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl
                                    ?? item.Product.Images?.FirstOrDefault()?.ImageUrl
                                    ?? "no-image.png";

                var mainImage = string.IsNullOrEmpty(imageFileName) ? "/images/no-image.png" : imageFileName;
                if (!mainImage.StartsWith("/") && !mainImage.StartsWith("http"))
                {
                    mainImage = "/images/" + mainImage;
                }

                decimal priceModifier = item.ProductVariant?.PriceModifier ?? 0m;
                decimal basePrice = item.Product.Price + priceModifier;
                decimal? discountPrice = item.Product.DiscountPrice.HasValue
                    ? item.Product.DiscountPrice.Value + priceModifier
                    : null;

                int stockQuantity = item.ProductVariant?.StockQuantity ?? item.Product.StockQuantity;

                viewModel.Items.Add(new WishlistItemDetailViewModel
                {
                    WishlistItemId = item.Id,
                    ProductId = item.ProductId,
                    ProductVariantId = item.ProductVariantId,
                    ProductName = item.Product.Name,
                    CategoryName = item.Product.ProductCategory?.Name,
                    VariantLabel = item.ProductVariant != null ? FormatVariantLabel(item.ProductVariant) : null,
                    Price = basePrice,
                    DiscountPrice = discountPrice,
                    ProductImagePath = mainImage,
                    StockQuantity = stockQuantity,
                    AddedAt = item.AddedAt
                });
            }

            return viewModel;
        }

        public async Task<(bool Success, string Message, WishlistViewModel? Wishlist)> AddToWishlistAsync(
            string sessionId, int productId, int? variantId = null)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                return (false, "M\u0259hsul tap\u0131lmad\u0131.", null);
            }

            if (!product.IsActive)
            {
                return (false, "Bu m\u0259hsul haz\u0131rda aktiv deyil.", null);
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.SessionId == sessionId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    SessionId = sessionId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            var existingItem = wishlist.WishlistItems
                .FirstOrDefault(wi => wi.ProductId == productId && wi.ProductVariantId == variantId);

            if (existingItem != null)
            {
                var currentWishlist = await GetWishlistAsync(sessionId);
                return (true, "M\u0259hsul art\u0131q ist\u0259k siyah\u0131n\u0131zdad\u0131r.", currentWishlist);
            }

            var newItem = new WishlistItem
            {
                WishlistId = wishlist.Id,
                ProductId = productId,
                ProductVariantId = variantId,
                AddedAt = DateTime.UtcNow
            };

            wishlist.WishlistItems.Add(newItem);
            wishlist.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var updatedWishlist = await GetWishlistAsync(sessionId);
            return (true, "M\u0259hsul ist\u0259k siyah\u0131s\u0131na \u0259lav\u0259 edildi.", updatedWishlist);
        }

        public async Task<(bool Success, string Message, WishlistViewModel? Wishlist)> RemoveFromWishlistAsync(
            string sessionId, int? wishlistItemId, int? productId = null)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.SessionId == sessionId);

            if (wishlist == null)
            {
                return (false, "\u0130st\u0259k siyah\u0131s\u0131 tap\u0131lmad\u0131.", null);
            }

            WishlistItem? item = null;
            if (wishlistItemId.HasValue && wishlistItemId.Value > 0)
            {
                item = wishlist.WishlistItems.FirstOrDefault(wi => wi.Id == wishlistItemId.Value);
            }
            else if (productId.HasValue && productId.Value > 0)
            {
                item = wishlist.WishlistItems.FirstOrDefault(wi => wi.ProductId == productId.Value);
            }

            if (item == null)
            {
                return (false, "M\u0259hsul ist\u0259k siyah\u0131s\u0131nda tap\u0131lmad\u0131.", null);
            }

            _context.WishlistItems.Remove(item);
            wishlist.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var updatedWishlist = await GetWishlistAsync(sessionId);
            return (true, "M\u0259hsul ist\u0259k siyah\u0131s\u0131ndan \u00E7\u0131xar\u0131ld\u0131.", updatedWishlist);
        }

        public async Task<int> GetWishlistCountAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) return 0;

            return await _context.WishlistItems
                .Where(wi => wi.Wishlist.SessionId == sessionId)
                .CountAsync();
        }

        private static string FormatVariantLabel(ProductVariant variant)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(variant.Size)) parts.Add($"\u00D6l\u00E7\u00FC: {variant.Size}");
            if (!string.IsNullOrWhiteSpace(variant.Weight)) parts.Add($"\u00C7\u0259ki: {variant.Weight}");
            if (!string.IsNullOrWhiteSpace(variant.Color)) parts.Add($"R\u0259ng: {variant.Color}");
            return string.Join(" | ", parts);
        }
    }
}