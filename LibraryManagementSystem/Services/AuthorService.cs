using LibraryManagementSystem.DTOs.Author;
using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

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
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Author name");

            var author = new Author
            {
                Id = Store.Authors.Any() ? Store.Authors.Max(a => a.Id) + 1 : 1,
                Name = dto.Name,
                Email = dto.Email
            };

            Store.Authors.Add(author);

            var result = author.Adapt<AuthorListDto>();
            result.BookCount = Store.Books.Count(b => b.AuthorId == author.Id);

            return result;
        }

        public List<AuthorListDto> ListAuthors()
        {
            var bookCounts = Store.Books
                .Where(b => !b.IsArchived)
                .GroupBy(b => b.AuthorId)
                .ToDictionary(g => g.Key, g => g.Count());

            var authors = Store.Authors.Where(a => !a.IsArchived);

            return authors
                .OrderBy(a => a.Name)
                .Select(a =>
                {
                    var dto = a.Adapt<AuthorListDto>();
                    dto.BookCount = bookCounts.TryGetValue(a.Id, out var count) ? count : 0;
                    return dto;
                })
                .ToList();
        }

        public bool EditAuthor(UpdateAuthorDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(dto.Id, "Id");
            Validate.NotEmpty(dto.Name, "Author name");

            var author = Store.Authors.FirstOrDefault(a => a.Id == dto.Id && !a.IsArchived);
            Validate.Exists(author, $"Author with id {dto.Id}");

            author!.Name = dto.Name;
            author.Email = dto.Email;

            return true;
        }

        public bool ArchiveAuthor(int id, int performedByUserId)
        {
            Validate.Positive(id, "Id");
            Validate.Positive(performedByUserId, "performedByUserId");

            var author = Store.Authors.FirstOrDefault(a => a.Id == id);
            Validate.Exists(author, $"Author with id {id}");

            if (author!.IsArchived)
                throw new ConflictException($"Author with id {id} is already archived.");

            foreach (var book in Store.Books.Where(b => b.AuthorId == id))
            {
                book.AuthorId = 0; // Unknown Author
            }

            author.Name = "Unknown";
            author.Email = string.Empty;

            author.IsArchived = true;
            author.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);
            author.ArchivedByUserId = performedByUserId;

            return true;
        }

    }
}
