using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using PetShop.DTOs;

namespace PetShop.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.ProductCategories.ToListAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                IconClass = c.IconClass,
                ParentCategoryId = c.ParentCategoryId
            });
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                IconClass = category.IconClass,
                ParentCategoryId = category.ParentCategoryId
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = new ProductCategory
            {
                Name = dto.Name,
                Slug = dto.Slug,
                IconClass = dto.IconClass,
                ParentCategoryId = dto.ParentCategoryId
            };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                IconClass = category.IconClass,
                ParentCategoryId = category.ParentCategoryId
            };
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null) return false;

            category.Name = dto.Name;
            category.Slug = dto.Slug;
            category.IconClass = dto.IconClass;
            category.ParentCategoryId = dto.ParentCategoryId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null) return false;

            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
