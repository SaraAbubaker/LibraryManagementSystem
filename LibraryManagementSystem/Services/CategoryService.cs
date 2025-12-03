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
        public CategoryDto CreateCategory(CreateCategoryDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Category name");

            var category = dto.Adapt<Category>();

            _categoryRepo.Add(category);

            return category.Adapt<CategoryDto>();
        }

        public List<CategoryDto> GetAllCategories()
        {
            return _categoryRepo.GetAll()
                .Where(c => !c.IsArchived)
                .Select(c => c.Adapt<CategoryDto>())
                .ToList();
        }

        public CategoryDto UpdateCategory(UpdateCategoryDto dto, int userId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Category name");
            Validate.Positive(dto.Id, "Id");

            var category = _categoryRepo.GetById(dto.Id);

            category!.Name = dto.Name;
            category.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            category.LastModifiedByUserId = userId;

            _categoryRepo.Update(category);

            return category.Adapt<CategoryDto>();
        }

        //Archives category & Moves books out of it
        public bool ArchiveCategory(int id, int? archivedByUserId = null)
        {
            Validate.Positive(id, "id");

            var category = _categoryRepo.GetById(id);

            if (category!.IsArchived)
                throw new ConflictException($"Category with id {id} is already archived.");

            foreach (var book in _bookRepo.GetAll().Where(b => b.CategoryId == id))
            {
                book.CategoryId = -1; //Unknown
                book.Category = _categoryRepo.GetById(0); //Unknown category
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
