using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.DTOs.Category;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class CategoryService
    {
        private readonly LibraryContext _context;

        public CategoryService(LibraryContext context)
        {
            _context = context;
        }

        //CRUD
        public CategoryDto CreateCategory(CreateCategoryDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Category name");

            var category = dto.Adapt<Category>();

            _context.Categories.Add(category);
            _context.SaveChanges();

            return category.Adapt<CategoryDto>();
        }

        public List<CategoryDto> GetAllCategories()
        {
            return _context.Categories
                .Where(c => !c.IsArchived)
                .Select(c => c.Adapt<CategoryDto>())
                .ToList();
        }

        public CategoryDto UpdateCategory(UpdateCategoryDto dto, int userId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Category name");
            Validate.Positive(dto.Id, "Id");

            var category = _context.Categories.FirstOrDefault(c => c.Id == dto.Id);
            Validate.Exists(category, $"Category with id {dto.Id}");

            category!.Name = dto.Name;
            category.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            category.LastModifiedByUserId = userId;

            _context.SaveChanges();

            return category.Adapt<CategoryDto>();
        }

        //Archives category & Moves books out of it
        public bool ArchiveCategory(int id, int? archivedByUserId = null)
        {
            Validate.Positive(id, "id");

            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            Validate.Exists(category, $"Category with id {id}");

            if (category!.IsArchived)
                throw new ConflictException($"Category with id {id} is already archived.");

            foreach (var book in _context.Books.Where(b => b.CategoryId == id))
            {
                book.CategoryId = -1; // Unknown
                book.Category = _context.Categories.First(c => c.Id == 0);
            }

            category.IsArchived = true;
            category.ArchivedByUserId = archivedByUserId;
            category.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            _context.SaveChanges();

            return true;
        }
    }
}
