using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.DTOs.Category;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class CategoryService
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
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Category name");

            var category = dto.Adapt<Category>();

            await _categoryRepo.AddAsync(category);

            return category.Adapt<CategoryDto>();
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();

            return categories
                .Where(c => !c.IsArchived)
                .Select(c => c.Adapt<CategoryDto>())
                .ToList();
        }

        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto dto, int userId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Category name");
            Validate.Positive(dto.Id, "Id");
            Validate.Positive(userId, nameof(userId));

            var category = await _categoryRepo.GetByIdAsync(dto.Id);

            category!.Name = dto.Name;
            category.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            category.LastModifiedByUserId = userId;

            _categoryRepo.Update(category);

            return category.Adapt<CategoryDto>();
        }

        //Archives category & Moves books out of it
        public async Task<bool> ArchiveCategoryAsync(int id, int? archivedByUserId = null)
        {
            Validate.Positive(id, "id");

            var category = await _categoryRepo.GetByIdAsync(id);

            if (category!.IsArchived)
                throw new ConflictException($"Category with id {id} is already archived.");

            var books = (await _bookRepo.GetAllAsync())
                .Where(b => b.CategoryId == id)
                .ToList();

            var unknownCategory = await _categoryRepo.GetByIdAsync(-1);

            foreach (var book in books)
            {
                book.CategoryId = -1; //Unknown
                book.Category = unknownCategory;
                _bookRepo.Update(book);
            }

            category.IsArchived = true;
            category.ArchivedByUserId = archivedByUserId;
            category.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            _categoryRepo.Update(category);

            return true;
        }
    }
}
