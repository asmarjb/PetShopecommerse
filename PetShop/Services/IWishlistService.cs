using PetShopEcommerse.Models;

namespace PetShop.Services
{
    public interface IWishlistService
    {
        string GetOrCreateSessionId();
        Task<WishlistViewModel> GetWishlistAsync(string sessionId);
        Task<(bool Success, string Message, WishlistViewModel? Wishlist)> AddToWishlistAsync(string sessionId, int productId, int? variantId = null);
        Task<(bool Success, string Message, WishlistViewModel? Wishlist)> RemoveFromWishlistAsync(string sessionId, int? wishlistItemId, int? productId = null);
        Task<int> GetWishlistCountAsync(string sessionId);
    }
}
