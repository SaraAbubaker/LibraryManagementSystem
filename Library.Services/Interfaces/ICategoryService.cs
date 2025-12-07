
using Library.Shared.DTOs.Category;

namespace Library.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, int userId);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto dto, int userId);
        Task<bool> ArchiveCategoryAsync(int id, int? performedByUserId = null);
    }
}
