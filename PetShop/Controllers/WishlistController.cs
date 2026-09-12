using Microsoft.AspNetCore.Mvc;
using PetShop.Services;
using PetShopEcommerse.Models;

namespace PetShopEcommerse.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // GET: /Wishlist
        // GET: /Wishlist/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sessionId = _wishlistService.GetOrCreateSessionId();
            var wishlist = await _wishlistService.GetWishlistAsync(sessionId);
            return View(wishlist);
        }

        // POST: /Wishlist/AddToWishlist
        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDto dto)
        {
            if (dto == null || dto.ProductId <= 0)
            {
                return BadRequest(new WishlistResponseDto
                {
                    Success = false,
                    Message = "M\u0259hsul d\u00FCzg\u00FCn t\u0259yin edilm\u0259yib."
                });
            }

            var sessionId = _wishlistService.GetOrCreateSessionId();
            var result = await _wishlistService.AddToWishlistAsync(sessionId, dto.ProductId, dto.ProductVariantId);

            if (!result.Success)
            {
                return BadRequest(new WishlistResponseDto
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new WishlistResponseDto
            {
                Success = true,
                Message = result.Message,
                TotalCount = result.Wishlist?.TotalItemCount ?? 0,
                Data = result.Wishlist
            });
        }

        // POST: /Wishlist/AddToWishlist/{productId}
        [HttpPost]
        [Route("Wishlist/AddToWishlist/{productId:int}")]
        public async Task<IActionResult> AddToWishlistById(int productId, [FromQuery] int? variantId = null)
        {
            return await AddToWishlist(new AddToWishlistDto
            {
                ProductId = productId,
                ProductVariantId = variantId
            });
        }

        // POST: /Wishlist/RemoveFromWishlist
        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist([FromBody] RemoveFromWishlistDto dto)
        {
            if (dto == null || (!dto.WishlistItemId.HasValue && !dto.ProductId.HasValue))
            {
                return BadRequest(new WishlistResponseDto
                {
                    Success = false,
                    Message = "Silin\u0259c\u0259k m\u0259hsul t\u0259yin edilm\u0259yib."
                });
            }

            var sessionId = _wishlistService.GetOrCreateSessionId();
            var result = await _wishlistService.RemoveFromWishlistAsync(sessionId, dto.WishlistItemId, dto.ProductId);

            if (!result.Success)
            {
                return BadRequest(new WishlistResponseDto
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new WishlistResponseDto
            {
                Success = true,
                Message = result.Message,
                TotalCount = result.Wishlist?.TotalItemCount ?? 0,
                Data = result.Wishlist
            });
        }

        // POST: /Wishlist/RemoveFromWishlist/{id}
        [HttpPost]
        [Route("Wishlist/RemoveFromWishlist/{id:int}")]
        public async Task<IActionResult> RemoveFromWishlistById(int id)
        {
            return await RemoveFromWishlist(new RemoveFromWishlistDto
            {
                WishlistItemId = id
            });
        }

        // GET: /Wishlist/GetWishlist
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var sessionId = _wishlistService.GetOrCreateSessionId();
            var wishlist = await _wishlistService.GetWishlistAsync(sessionId);

            return Ok(new WishlistResponseDto
            {
                Success = true,
                Message = "\u0130st\u0259k siyah\u0131s\u0131 \u0259ld\u0259 edildi.",
                TotalCount = wishlist.TotalItemCount,
                Data = wishlist
            });
        }
    }
}