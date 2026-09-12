using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using PetShopEcommerse.Models;

namespace PetShop.Services
{
    public class BasketService : IBasketService
    {
        private const string CartSessionCookieName = "petshop_cart_session";
        private const string CartSessionKey = "CartSessionId";

        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BasketService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
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

            // 1) Cookie-ni yoxla
            if (httpContext.Request.Cookies.TryGetValue(CartSessionCookieName, out var cookieSessionId) &&
                !string.IsNullOrWhiteSpace(cookieSessionId))
            {
                httpContext.Session.SetString(CartSessionKey, cookieSessionId);
                return cookieSessionId;
            }

            // 2) Session-u yoxla
            var sessionSessionId = httpContext.Session.GetString(CartSessionKey);
            if (!string.IsNullOrWhiteSpace(sessionSessionId))
            {
                AppendSessionCookie(httpContext, sessionSessionId);
                return sessionSessionId;
            }

            // 3) Yeni unikal GUID yarat və həm cookie, həm də session-a yaz
            var newSessionId = Guid.NewGuid().ToString("N");
            httpContext.Session.SetString(CartSessionKey, newSessionId);
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

            httpContext.Response.Cookies.Append(CartSessionCookieName, sessionId, cookieOptions);
        }

        public async Task<BasketViewModel> GetBasketAsync(string sessionId)
        {
            var viewModel = new BasketViewModel();
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return viewModel;
            }

            var cart = await _context.Carts
                .AsNoTracking()
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Images)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null || !cart.CartItems.Any())
            {
                return viewModel;
            }

            foreach (var item in cart.CartItems)
            {
                if (item.Product == null) continue;

                var currentPrice = CalculateCurrentUnitPrice(item.Product, item.ProductVariant);
                var mainImage = item.Product.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl
                                ?? item.Product.Images?.FirstOrDefault()?.ImageUrl
                                ?? "no-image.png";

                if (!mainImage.StartsWith("/") && !mainImage.StartsWith("http"))
                {
                    mainImage = "/images/" + mainImage;
                }

                int availableStock = item.ProductVariant?.StockQuantity ?? item.Product.StockQuantity;

                viewModel.Items.Add(new BasketItemDetailViewModel
                {
                    CartItemId = item.Id,
                    ProductId = item.ProductId,
                    ProductVariantId = item.ProductVariantId,
                    ProductName = item.Product.Name,
                    VariantLabel = item.ProductVariant != null ? FormatVariantLabel(item.ProductVariant) : null,
                    UnitPrice = item.UnitPriceSnapshot,
                    CurrentPrice = currentPrice,
                    Quantity = item.Quantity,
                    ProductImagePath = mainImage,
                    AvailableStock = availableStock
                });
            }

            return viewModel;
        }

        public async Task<(bool Success, string Message, BasketViewModel? Cart, int RemainingStock)> AddToCartAsync(
            string sessionId, int productId, int? variantId, int quantity)
        {
            if (quantity <= 0)
            {
                return (false, "Sifariş olunan say ən azı 1 olmalıdır.", null, 0);
            }

            var product = await _context.Products
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return (false, "Məhsul tapılmadı.", null, 0);
            }

            if (!product.IsActive)
            {
                return (false, "Bu məhsul hazırda satışda deyil.", null, 0);
            }

            ProductVariant? variant = null;
            if (variantId.HasValue && variantId.Value > 0)
            {
                variant = product.Variants.FirstOrDefault(v => v.Id == variantId.Value);
                if (variant == null)
                {
                    return (false, "Seçilmiş məhsul variantı tapılmadı.", null, 0);
                }
            }

            // Mövcud qalan stok yoxlanışı
            int availableStock = variant?.StockQuantity ?? product.StockQuantity;
            if (availableStock <= 0)
            {
                return (false, "Bu məhsulun stoku bitmişdir.", null, 0);
            }

            if (quantity > availableStock)
            {
                return (false, $"Kifayət qədər stok yoxdur. Qalan stok: {availableStock} ədəd.", null, availableStock);
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null)
            {
                cart = new Cart
                {
                    SessionId = sessionId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId && ci.ProductVariantId == variantId);

            decimal unitPriceSnapshot = (product.DiscountPrice ?? product.Price) + (variant?.PriceModifier ?? 0m);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.UnitPriceSnapshot = unitPriceSnapshot;
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    ProductVariantId = variantId,
                    Quantity = quantity,
                    UnitPriceSnapshot = unitPriceSnapshot,
                    AddedAt = DateTime.UtcNow
                };
                cart.CartItems.Add(newItem);
            }

            // Səbətə əlavə edildikcə bazadakı stok sayını azaldırıq (həm əsas məhsul, həm də variant sinxron saxlanılır)
            if (variant != null)
            {
                variant.StockQuantity = Math.Max(0, variant.StockQuantity - quantity);
                product.StockQuantity = Math.Max(0, product.StockQuantity - quantity);
            }
            else
            {
                product.StockQuantity = Math.Max(0, product.StockQuantity - quantity);
                if (product.Variants != null && product.Variants.Any())
                {
                    foreach (var v in product.Variants)
                    {
                        v.StockQuantity = Math.Max(0, v.StockQuantity - quantity);
                    }
                }
            }

            int remainingStock = variant?.StockQuantity ?? product.StockQuantity;

            cart.UpdatedAt = DateTime.UtcNow;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            var updatedCart = await GetBasketAsync(sessionId);
            return (true, "Məhsul uğurla səbətə əlavə edildi.", updatedCart, remainingStock);
        }

        public async Task<(bool Success, string Message, BasketViewModel? Cart)> UpdateQuantityAsync(
            string sessionId, int cartItemId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Variants)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null)
            {
                return (false, "Səbət tapılmadı.", null);
            }

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null)
            {
                return (false, "Səbətdə belə bir məhsul tapılmadı.", null);
            }

            int oldQuantity = item.Quantity;
            int diff = quantity - oldQuantity;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1) Say 0 və ya mənfi olarsa səbətdən sil və stoku tam geri qaytar
                if (quantity <= 0)
                {
                    if (item.ProductVariant != null)
                    {
                        item.ProductVariant.StockQuantity += oldQuantity;
                    }
                    if (item.Product != null)
                    {
                        item.Product.StockQuantity += oldQuantity;
                        if (item.Product.Variants != null && item.ProductVariant == null)
                        {
                            foreach (var v in item.Product.Variants)
                            {
                                v.StockQuantity += oldQuantity;
                            }
                        }
                    }

                    _context.CartItems.Remove(item);
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var cartAfterRemoval = await GetBasketAsync(sessionId);
                    return (true, "Məhsul səbətdən silindi.", cartAfterRemoval);
                }

                // 2) Say artırılırsa (diff > 0): stok kifayət edirmi yoxla və stoku azalt
                if (diff > 0)
                {
                    int availableStock = item.ProductVariant?.StockQuantity ?? item.Product?.StockQuantity ?? 0;
                    if (diff > availableStock)
                    {
                        await transaction.RollbackAsync();
                        return (false, $"Kifayət qədər stok yoxdur. Əlavə edilə biləcək maksimum say: {availableStock} ədəd.", null);
                    }

                    if (item.ProductVariant != null)
                    {
                        item.ProductVariant.StockQuantity -= diff;
                    }
                    if (item.Product != null)
                    {
                        item.Product.StockQuantity = Math.Max(0, item.Product.StockQuantity - diff);
                        if (item.Product.Variants != null && item.ProductVariant == null)
                        {
                            foreach (var v in item.Product.Variants)
                            {
                                v.StockQuantity = Math.Max(0, v.StockQuantity - diff);
                            }
                        }
                    }
                }
                // 3) Say azaldılırsa (diff < 0): fərq qədər stoku bazaya geri qaytar
                else if (diff < 0)
                {
                    int returnQty = -diff;
                    if (item.ProductVariant != null)
                    {
                        item.ProductVariant.StockQuantity += returnQty;
                    }
                    if (item.Product != null)
                    {
                        item.Product.StockQuantity += returnQty;
                        if (item.Product.Variants != null && item.ProductVariant == null)
                        {
                            foreach (var v in item.Product.Variants)
                            {
                                v.StockQuantity += returnQty;
                            }
                        }
                    }
                }

                item.Quantity = quantity;
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            var updatedCart = await GetBasketAsync(sessionId);
            return (true, "Səbət yeniləndi.", updatedCart);
        }

        public async Task<(bool Success, string Message, BasketViewModel? Cart, int? RemovedProductId, int RestoredStock)> RemoveFromCartAsync(
            string sessionId, int? cartItemId, int? productId = null, int? variantId = null)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Variants)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null)
            {
                return (false, "Səbət tapılmadı.", null, null, 0);
            }

            CartItem? item = null;
            if (cartItemId.HasValue && cartItemId.Value > 0)
            {
                item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId.Value);
            }
            else if (productId.HasValue && productId.Value > 0)
            {
                item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId.Value && ci.ProductVariantId == variantId);
            }

            if (item == null)
            {
                return (false, "Məhsul səbətdə tapılmadı.", null, null, 0);
            }

            int removedProdId = item.ProductId;
            int restoredStock = 0;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Səbətdən silinən məhsulun sayını stoka geri qaytarırıq (həm variant, həm də əsas məhsul sinxron qalır)
                if (item.ProductVariant != null)
                {
                    item.ProductVariant.StockQuantity += item.Quantity;
                }
                if (item.Product != null)
                {
                    item.Product.StockQuantity += item.Quantity;
                    if (item.Product.Variants != null && item.ProductVariant == null)
                    {
                        foreach (var v in item.Product.Variants)
                        {
                            v.StockQuantity += item.Quantity;
                        }
                    }
                }

                restoredStock = item.ProductVariant?.StockQuantity ?? item.Product?.StockQuantity ?? 0;

                _context.CartItems.Remove(item);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            var updatedCart = await GetBasketAsync(sessionId);
            return (true, "Məhsul səbətdən çıxarıldı.", updatedCart, removedProdId, restoredStock);
        }

        public async Task MigrateCartAsync(string sessionId, string userId)
        {
            if (string.IsNullOrWhiteSpace(sessionId) || string.IsNullOrWhiteSpace(userId)) return;

            var guestCart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.UserId == null);

            if (guestCart == null || !guestCart.CartItems.Any()) return;

            var userCart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (userCart == null)
            {
                guestCart.UserId = userId;
                guestCart.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                foreach (var guestItem in guestCart.CartItems.ToList())
                {
                    var existingUserItem = userCart.CartItems
                        .FirstOrDefault(i => i.ProductId == guestItem.ProductId && i.ProductVariantId == guestItem.ProductVariantId);

                    if (existingUserItem != null)
                    {
                        existingUserItem.Quantity += guestItem.Quantity;
                    }
                    else
                    {
                        guestItem.CartId = userCart.Id;
                        userCart.CartItems.Add(guestItem);
                    }
                }

                userCart.UpdatedAt = DateTime.UtcNow;
                _context.Carts.Remove(guestCart);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> CleanupOldCartsAsync(int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);

            var staleGuestCarts = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                .Where(c => c.UserId == null && (c.UpdatedAt ?? c.CreatedAt) < cutoffDate)
                .ToListAsync();

            if (staleGuestCarts.Any())
            {
                foreach (var cart in staleGuestCarts)
                {
                    foreach (var item in cart.CartItems)
                    {
                        if (item.ProductVariant != null)
                        {
                            item.ProductVariant.StockQuantity += item.Quantity;
                        }
                        if (item.Product != null)
                        {
                            item.Product.StockQuantity += item.Quantity;
                        }
                    }
                }

                _context.Carts.RemoveRange(staleGuestCarts);
                return await _context.SaveChangesAsync();
            }

            return 0;
        }

        private static decimal CalculateCurrentUnitPrice(Product product, ProductVariant? variant)
        {
            var basePrice = product.DiscountPrice ?? product.Price;
            return basePrice + (variant?.PriceModifier ?? 0m);
        }

        private static string FormatVariantLabel(ProductVariant variant)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(variant.Size)) parts.Add($"Ölçü: {variant.Size}");
            if (!string.IsNullOrWhiteSpace(variant.Weight)) parts.Add($"Çəki: {variant.Weight}");
            if (!string.IsNullOrWhiteSpace(variant.Color)) parts.Add($"Rəng: {variant.Color}");

            return parts.Count > 0 ? string.Join(", ", parts) : "Variant";
        }

        public async Task ApplyCouponAndShippingAsync(BasketViewModel basket, string? couponCode)
        {
            if (basket == null || basket.IsEmpty)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code.ToLower() == couponCode.Trim().ToLower() && c.IsActive);

                if (coupon != null)
                {
                    bool isExpired = coupon.ExpiryDate.HasValue && coupon.ExpiryDate.Value < DateTime.UtcNow;
                    bool isLimitReached = coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value;
                    bool isMinNotMet = coupon.MinimumOrderAmount.HasValue && basket.TotalPrice < coupon.MinimumOrderAmount.Value;

                    if (!isExpired && !isLimitReached && !isMinNotMet)
                    {
                        basket.AppliedCouponCode = coupon.Code;
                        if (coupon.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
                        {
                            basket.DiscountPercent = coupon.DiscountValue;
                            basket.DiscountAmount = Math.Round(basket.TotalPrice * (coupon.DiscountValue / 100m), 2);
                        }
                        else
                        {
                            basket.DiscountAmount = Math.Min(basket.TotalPrice, coupon.DiscountValue);
                        }

                        // Maksimum endirim məbləği limiti
                        if (coupon.MaxDiscountAmount.HasValue && basket.DiscountAmount > coupon.MaxDiscountAmount.Value)
                        {
                            basket.DiscountAmount = coupon.MaxDiscountAmount.Value;
                        }
                    }
                }
            }
        }

        public async Task ClearBasketAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) return;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
