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
        private readonly LibraryDataStore Store;
        //Injection
        public AuthorService(LibraryDataStore store)
        {
            Store = store;
        }

        //CRUD
        public AuthorListDto CreateAuthor(CreateAuthorDto dto)
        {
            var author = new Author
            {
                Id = Store.Authors.Count + 1,
                Name = dto.Name,
                Email = dto.Email
            };

            Store.Authors.Add(author);

            var result = author.Adapt<AuthorListDto>();
            result.BookCount = Store.Books.Count(b => b.AuthorId == author.Id);

            return result;
        }
        
        public List<AuthorListDto> ListAuthors(bool includeArchived = false)
        {
            var authors = includeArchived
                ? Store.Authors.AsEnumerable()
                : Store.Authors.Where(a => !a.IsArchived);

            return authors
                .OrderBy(a => a.Name)
                .Select(a =>
                {
                    var dto = a.Adapt<AuthorListDto>();
                    dto.BookCount = Store.Books.Count(b => b.AuthorId == a.Id);
                    return dto;
                })
                .ToList();
        }

        public List<BookListDto> GetBooksByAuthor(int authorId, bool includeArchivedBooks = false)
        {
            var author = Store.Authors.FirstOrDefault(a => a.Id == authorId);
            if (author == null)
                throw new KeyNotFoundException($"Author with ID {authorId} not found.");

            if (author.IsArchived)
                throw new InvalidOperationException("Author is archived.");

            var books = Store.Books
                .Where(b => b.AuthorId == authorId && (includeArchivedBooks || !b.IsArchived))
                .OrderBy(b => b.Title)
                .ToList();

            return books.Select(b =>
            {
                var dto = b.Adapt<BookListDto>();

                dto.AuthorName = Store.Authors
                    .FirstOrDefault(x => x.Id == b.AuthorId && !x.IsArchived)
                    ?.Name ?? "Unknown";

                dto.CategoryName = Store.Categories
                    .FirstOrDefault(c => c.Id == b.CategoryId && !c.IsArchived)
                    ?.Name ?? "Unknown";

                dto.IsAvailable = Store.InventoryRecords
                    .Where(r => r.BookId == b.Id)
                    .Any(r => r.IsAvailable);

                return dto;
            })
            .ToList();
        }
        
        public AuthorListDto EditAuthor(int id, UpdateAuthorDto dto)
        {
            var author = Store.Authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                throw new KeyNotFoundException($"Author with ID {id} not found.");

            if (author.IsArchived)
                throw new InvalidOperationException("Cannot edit an archived author.");

            author.Name = dto.Name;
            author.Email = dto.Email;

            var result = author.Adapt<AuthorListDto>();
            result.BookCount = Store.Books.Count(b => b.AuthorId == author.Id);

            return result;
        }

        public bool ArchiveAuthor(int id, int performedByUserId)
        {
            var author = Store.Authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                return false;

            foreach (var book in Store.Books.Where(b => b.AuthorId == id))
            {
                book.AuthorId = 0; // Unknown Author
            }

            author.Name = "Unknown";
            author.Email = string.Empty;

            author.IsArchived = true;
            author.ArchivedDate = DateTime.Now;
            author.ArchivedByUserId = performedByUserId;

            return true;
        }

        public AuthorListDto RestoreAuthorArchive(int id, int performedByUserId = 0)
        {
            var author = Store.Authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                throw new KeyNotFoundException($"Author with ID {id} not found.");

            if (!author.IsArchived)
                throw new InvalidOperationException("Author is not archived.");

            // restore
            author.IsArchived = false;
            author.ArchivedDate = null;
            author.ArchivedByUserId = null;

            author.LastModifiedByUserId = performedByUserId;
            author.LastModifiedDate = DateTime.Now;

            var dto = author.Adapt<AuthorListDto>();
            dto.BookCount = Store.Books.Count(b => b.AuthorId == author.Id);

            return dto;
        }

    }

}
