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

            category.CreatedByUserId = userId;
            category.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            category.LastModifiedByUserId = userId;
            category.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            category.IsArchived = false;

            await _categoryRepo.AddAsync(category);

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
                $"Category with id {dto.Id}"
            );

            category.Name = dto.Name;
            category.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            category.LastModifiedByUserId = userId;

            await _categoryRepo.UpdateAsync(category);

            return category.Adapt<CategoryDto>();
        }

        //Archives category & Moves books out of it
        public async Task<bool> ArchiveCategoryAsync(int id, int? performedByUserId = null)
        {
            Validate.Positive(id, nameof(id));

            var category = Validate.Exists(
                await _categoryRepo.GetById(id).FirstOrDefaultAsync(),
                $"Category with id {id}"
            );

            if (category.IsArchived)
                throw new ConflictException($"Category with id {id} is already archived.");

            var books =  _bookRepo.GetAll().Where(b => b.CategoryId == id);

            var unknownCategory = await _categoryRepo.GetById(-1).FirstOrDefaultAsync();

            foreach (var book in books)
            {
                book.CategoryId = -1; //Unknown
                book.Category = unknownCategory;
                await _bookRepo.UpdateAsync(book);
            }

            category.IsArchived = true;
            category.ArchivedByUserId = performedByUserId;
            category.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);
            category.LastModifiedByUserId = performedByUserId;
            category.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _categoryRepo.UpdateAsync(category);

            return true;
        }
    }
}
