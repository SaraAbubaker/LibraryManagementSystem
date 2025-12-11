using Library.Domain.Repositories;
using Library.Shared.Helpers;
using Library.Shared.DTOs.Category;
using Library.Shared.Exceptions;
using Library.Entities.Models;
using Library.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Library.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Book> _bookRepo;

        public CategoryService(
            IGenericRepository<Category> categoryRepo,
            IGenericRepository<Book> bookRepo)
        {
            _categoryRepo = categoryRepo;
            _bookRepo = bookRepo;
        }


        //CRUD
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, int userId)
        {
            Validate.ValidateModel(dto);
            Validate.Positive(userId, nameof(userId));

            var category = dto.Adapt<Category>();

            await _categoryRepo.AddAsync(category, userId);
            await _categoryRepo.CommitAsync();

            return category.Adapt<CategoryDto>();
        }

        public IQueryable<CategoryDto> GetAllCategoriesQuery()
        {
            return _categoryRepo.GetAll()
                .AsNoTracking()
                .Select(c => c.Adapt<CategoryDto>());
        }

        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto dto, int userId)
        {
            Validate.ValidateModel(dto);
            Validate.Positive(userId, nameof(userId));

            var category = Validate.Exists(
                await _categoryRepo.GetById(dto.Id).FirstOrDefaultAsync(),
                dto.Id
            );

            category.Name = dto.Name;
            await _categoryRepo.UpdateAsync(category, userId);
            await _categoryRepo.CommitAsync();

            return category.Adapt<CategoryDto>();
        }

        //Archives category & Moves books out of it
        public async Task<bool> ArchiveCategoryAsync(int id, int performedByUserId)
        {
            Validate.Positive(id, nameof(id));

            var category = Validate.Exists(
                await _categoryRepo.GetById(id).FirstOrDefaultAsync(),
                id
            );

            if (category.IsArchived)
                throw new ConflictException($"Category with id {id} is already archived.");

            var books =  _bookRepo.GetAll().Where(b => b.CategoryId == id);

            var unknownCategory = await _categoryRepo.GetById(-1).FirstOrDefaultAsync();

            foreach (var book in books)
            {
                book.CategoryId = -1; //Unknown
                book.Category = unknownCategory;
                await _bookRepo.UpdateAsync(book, performedByUserId);
            }

            await _categoryRepo.ArchiveAsync(category, performedByUserId);
            await _categoryRepo.CommitAsync();

            return true;
        }
    }
}
