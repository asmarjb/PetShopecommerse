using PetShop.DTOs;

namespace PetShop.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
