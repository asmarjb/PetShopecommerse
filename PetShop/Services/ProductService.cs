using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using PetShop.DTOs;

namespace PetShop.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.ProductCategory)
                .ToListAsync();

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                StockQuantity = p.StockQuantity,
                Sku = p.Sku,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                ProductCategoryId = p.ProductCategoryId,
                BrandId = p.BrandId,
                Category = p.ProductCategory != null ? new CategoryDto
                {
                    Id = p.ProductCategory.Id,
                    Name = p.ProductCategory.Name,
                    Slug = p.ProductCategory.Slug,
                    IconClass = p.ProductCategory.IconClass,
                    ParentCategoryId = p.ProductCategory.ParentCategoryId
                } : null
            });
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                StockQuantity = product.StockQuantity,
                Sku = product.Sku,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                ProductCategoryId = product.ProductCategoryId,
                BrandId = product.BrandId,
                Category = product.ProductCategory != null ? new CategoryDto
                {
                    Id = product.ProductCategory.Id,
                    Name = product.ProductCategory.Name,
                    Slug = product.ProductCategory.Slug,
                    IconClass = product.ProductCategory.IconClass,
                    ParentCategoryId = product.ProductCategory.ParentCategoryId
                } : null
            };
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DiscountPrice = dto.DiscountPrice,
                StockQuantity = dto.StockQuantity,
                Sku = dto.Sku,
                IsActive = dto.IsActive,
                ProductCategoryId = dto.ProductCategoryId,
                BrandId = dto.BrandId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(product.Id) ?? throw new InvalidOperationException("Failed to load created product.");
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.DiscountPrice = dto.DiscountPrice;
            product.StockQuantity = dto.StockQuantity;
            product.Sku = dto.Sku;
            product.IsActive = dto.IsActive;
            product.ProductCategoryId = dto.ProductCategoryId;
            product.BrandId = dto.BrandId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
