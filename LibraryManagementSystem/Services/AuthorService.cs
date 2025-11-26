using LibraryManagementSystem.DTOs.Author;
using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Linq;

namespace LibraryManagementSystem.Services
{
    public class AuthorService
    {
        private readonly List<Author> Authors;
        private readonly List<Book> Books;
        private readonly List<Category> Categories;
        private readonly List<InventoryRecord> Inventory;

        public AuthorService(
            List<Author> authors,
            List<Book> books,
            List<Category> categories,
            List<InventoryRecord> inventoryRecords)
        {
            Authors = authors;
            Books = books;
            Categories = categories;
            Inventory = inventoryRecords;
        }

        public List<AuthorListDto> ListAuthors()
        {
            return Authors
                .OrderBy(a => a.Name)
                .Select(a =>
                {
                    var dto = a.Adapt<AuthorListDto>();
                    dto.BookCount = Books.Count(b => b.AuthorId == a.Id);
                    return dto;
                })
                .ToList();
        }

        public List<BookListDto> GetBooksByAuthor(int authorId)
        {
            var books = Books
                .Where(b => b.AuthorId == authorId)
                .OrderBy(b => b.Title)
                .ToList();

            return books.Select(b =>
            {
                var dto = b.Adapt<BookListDto>();

                dto.AuthorName = Authors
                    .FirstOrDefault(x => x.Id == b.AuthorId)
                    ?.Name ?? "Unknown";

                dto.CategoryName = Categories
                    .FirstOrDefault(c => c.Id == b.CategoryId)
                    ?.Name ?? "Unknown";

                dto.IsAvailable = Inventory
                    .Where(r => r.BookId == b.Id)
                    .Any(r => r.IsAvailable);

                return dto;
            })
            .ToList();
        }

        public AuthorListDto AddAuthor(CreateAuthordDto dto)
        {
            var author = new Author
            {
                Id = Authors.Count + 1,
                Name = dto.Name,
                Email = dto.Email
            };

            Authors.Add(author);

            var result = author.Adapt<AuthorListDto>();
            result.BookCount = Books.Count(b => b.AuthorId == author.Id);

            return result;
        }


        public AuthorListDto? EditAuthor(int id, UpdateAuthorDto dto)
        {
            var author = Authors.FirstOrDefault(a => a.Id == id);
            if (author == null) return null;

            author.Name = dto.Name;
            author.Email = dto.Email;

            var result = author.Adapt<AuthorListDto>();
            result.BookCount = Books.Count(b => b.AuthorId == author.Id);

            return result;
        }


        public bool DeleteAuthor(int id)
        {
            var author = Authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                return false;

            Authors.Remove(author);
            return true;
        }
    }
}
