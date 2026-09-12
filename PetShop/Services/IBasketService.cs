using PetShopEcommerse.Models;

namespace PetShop.Services
{
    public interface IBasketService
    {
        string GetOrCreateSessionId();
        Task<BasketViewModel> GetBasketAsync(string sessionId);
        Task<(bool Success, string Message, BasketViewModel? Cart, int RemainingStock)> AddToCartAsync(string sessionId, int productId, int? variantId, int quantity);
        Task<(bool Success, string Message, BasketViewModel? Cart)> UpdateQuantityAsync(string sessionId, int cartItemId, int quantity);
        Task<(bool Success, string Message, BasketViewModel? Cart, int? RemovedProductId, int RestoredStock)> RemoveFromCartAsync(string sessionId, int? cartItemId, int? productId = null, int? variantId = null);
        Task MigrateCartAsync(string sessionId, string userId);
        Task<int> CleanupOldCartsAsync(int days = 30);
        Task ApplyCouponAndShippingAsync(BasketViewModel basket, string? couponCode);
        Task ClearBasketAsync(string sessionId);
    }
}
