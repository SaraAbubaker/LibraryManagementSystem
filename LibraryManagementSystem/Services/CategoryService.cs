using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.DTOs.Category;
using LibraryManagementSystem.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class CategoryService
    {
        private readonly LibraryDataStore Store;

        public CategoryService(LibraryDataStore store)
        {
            Store = store;
        }

        //CRUD
        public CategoryDto CreateCategory(CreateCategoryDto dto)
        {
            var category = dto.Adapt<Category>();

            category.Id = Store.Categories.Count + 1;

            Store.Categories.Add(category);

            return category.Adapt<CategoryDto>();
        }

        public List<CategoryDto> GetAllCategories()
        {
            return Store.Categories
                .Where(c => !c.IsArchived)
                .Select(c => c.Adapt<CategoryDto>())
                .ToList();
        }

        public List<BookListDto> ListBooksByCategory(int categoryId)
        {
            var category = Store.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (category == null || category.IsArchived)
                categoryId = 0; // unknown

            var books = Store.Books
                .Where(b => b.CategoryId == categoryId)
                .ToList();

            return books.Adapt<List<BookListDto>>();
        }

        public CategoryDto? UpdateCategory(UpdateCategoryDto dto, int UserId)
        {
            if (dto.Id == 0)
                return null;

            var category = Store.Categories.FirstOrDefault(c => c.Id == dto.Id);
            if (category == null)
                return null;

            category.Name = dto.Name;

            category.LastModifiedDate = DateTime.Now;
            category.LastModifiedByUserId = UserId;

            return category.Adapt<CategoryDto>();
        }

        //Archives category & Moves books out of it
        public bool ArchiveCategory(int id, int? archivedByUserId = null)
        {
            if (id == 0)
                return false;

            var category = Store.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null || category.IsArchived)
                return false;

            foreach (var book in Store.Books.Where(b => b.CategoryId == id))
            {
                book.CategoryId = 0; // Unknown
                book.Category = Store.Categories.First(c => c.Id == 0);
            }

            category.IsArchived = true;
            category.ArchivedByUserId = archivedByUserId;
            category.ArchivedDate = DateTime.Now;

            return true;
        }

        //Unarchives category but doesn't move books back
        public CategoryDto? UnarchiveCategory(int id, int userId)
        {
            if (id == 0)
                return null;

            var category = Store.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null || !category.IsArchived)
                return null;

            category.IsArchived = false;
            category.ArchivedByUserId = null;
            category.ArchivedDate = null;

            category.LastModifiedByUserId = userId;
            category.LastModifiedDate = DateTime.Now;

            return category.Adapt<CategoryDto>();
        }

    }
}
