
using Library.Shared.DTOs.Category;

namespace Library.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, int userId);
        IQueryable<CategoryDto> GetAllCategoriesQuery();
        Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto dto, int userId);
        Task<bool> ArchiveCategoryAsync(int id, int performedByUserId);
    }
}
