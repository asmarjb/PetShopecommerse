using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.Models;
using PetShop.Services;

namespace PetShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWishlistService _wishlistService;

        public ShopController(AppDbContext context, IWishlistService wishlistService)
        {
            _context = context;
            _wishlistService = wishlistService;
        }

        [Route("Shop")]
        [Route("Shop/Index")]
        [Route("products/{categorySlug?}")]
        public async Task<IActionResult> Index(
            string? categorySlug,
            string? searchTerm,
            int? categoryId,
            int? animalTypeId,
            int? brandId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            bool inStock = false,
            bool onlyDiscounted = false,
            int page = 1,
            int pageSize = 9)
        {
            // If categorySlug is provided via route (e.g., from home page /products/food)
            if (!string.IsNullOrWhiteSpace(categorySlug) && !categoryId.HasValue)
            {
                var matchedCategory = await _context.ProductCategories
                    .FirstOrDefaultAsync(c => c.Slug.ToLower() == categorySlug.ToLower());
                if (matchedCategory != null)
                {
                    categoryId = matchedCategory.Id;
                }
            }

            // Eager loading related entities
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductCategory)
                .Include(p => p.Images)
                .Include(p => p.ProductAnimalTypes)
                    .ThenInclude(pat => pat.AnimalType)
                .Where(p => p.IsActive);

            // 1. Search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term) ||
                                         (p.Description != null && p.Description.ToLower().Contains(term)));
            }

            // 2. Category filter
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.ProductCategoryId == categoryId.Value ||
                                         p.ProductCategory.ParentCategoryId == categoryId.Value);
            }

            // 3. Animal type filter
            if (animalTypeId.HasValue && animalTypeId.Value > 0)
            {
                query = query.Where(p => p.ProductAnimalTypes.Any(pat => pat.AnimalTypeId == animalTypeId.Value));
            }

            // 4. Brand filter
            if (brandId.HasValue && brandId.Value > 0)
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }

            // 5. Price filter
            if (minPrice.HasValue && minPrice.Value > 0)
            {
                query = query.Where(p => (p.DiscountPrice ?? p.Price) >= minPrice.Value);
            }
            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                query = query.Where(p => (p.DiscountPrice ?? p.Price) <= maxPrice.Value);
            }

            // 6. In-stock filter
            if (inStock)
            {
                query = query.Where(p => p.StockQuantity > 0);
            }

            // 7. Only discounted filter
            if (onlyDiscounted)
            {
                query = query.Where(p => p.DiscountPrice.HasValue && p.DiscountPrice.Value < p.Price);
            }

            // 8. Sorting
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.DiscountPrice ?? p.Price),
                "price_desc" => query.OrderByDescending(p => p.DiscountPrice ?? p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "discount" => query.OrderByDescending(p => p.DiscountPrice.HasValue).ThenBy(p => p.DiscountPrice ?? p.Price),
                _ => query.OrderByDescending(p => p.CreatedAt) // Default: newest first
            };

            // Total count before pagination
            var totalItems = await query.CountAsync();

            // Pagination
            if (page < 1) page = 1;
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new ShopViewModel
            {
                Products = products,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                Categories = await _context.ProductCategories.OrderBy(c => c.Name).ToListAsync(),
                AnimalTypes = await _context.AnimalTypes.OrderBy(a => a.Name).ToListAsync(),
                Brands = await _context.Brands.OrderBy(b => b.Name).ToListAsync(),
                SearchTerm = searchTerm,
                SelectedCategoryId = categoryId,
                SelectedAnimalTypeId = animalTypeId,
                SelectedBrandId = brandId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                OnlyInStock = inStock,
                OnlyDiscounted = onlyDiscounted
            };

            // Load wishlisted product IDs for current session so the view can highlight hearts
            var sessionId = _wishlistService.GetOrCreateSessionId();
            var wishlist = await _wishlistService.GetWishlistAsync(sessionId);
            viewModel.WishlistProductIds = wishlist.Items
                .Select(i => i.ProductId)
                .ToHashSet();

            return View(viewModel);
        }
    }
}
