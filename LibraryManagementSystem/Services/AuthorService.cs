using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs.Author;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem.Services
{
    public class AuthorService
    {
        private readonly LibraryContext _context;

        public AuthorService(LibraryContext context)
        {
            _context = context;
        }

        //CRUD
        public AuthorListDto CreateAuthor(CreateAuthorDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Author name");

            var author = new Author
            {
                Name = dto.Name,
                Email = dto.Email
            };

            _context.Authors.Add(author);
            _context.SaveChanges();

            var result = author.Adapt<AuthorListDto>();

            result.BookCount = _context.Books.Count(b => b.AuthorId == author.Id);

            return result;
        }

        public List<AuthorListDto> ListAuthors()
        {
            var bookCounts = _context.Books
                .Where(b => !b.IsArchived)
                .GroupBy(b => b.AuthorId)
                .ToDictionary(g => g.Key, g => g.Count());

            var authors = _context.Authors
                .Where(a => !a.IsArchived)
                .OrderBy(a => a.Name)
                .ToList();

            return authors
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

            var author = _context.Authors.FirstOrDefault(a => a.Id == dto.Id && !a.IsArchived);
            Validate.Exists(author, $"Author with id {dto.Id}");

            author!.Name = dto.Name;
            author.Email = dto.Email;

            _context.SaveChanges();
            return true;
        }

        public bool ArchiveAuthor(int id, int performedByUserId)
        {
            Validate.Positive(id, "Id");
            Validate.Positive(performedByUserId, "performedByUserId");

            var author = _context.Authors.FirstOrDefault(a => a.Id == id);
            Validate.Exists(author, $"Author with id {id}");

            if (author!.IsArchived)
                throw new ConflictException($"Author with id {id} is already archived.");

            var books = _context.Books.Where(b => b.AuthorId == id).ToList();
            foreach (var book in books)
            {
                book.AuthorId = 0; // Unknown Author
            }

            author.Name = "Unknown";
            author.Email = string.Empty;
            author.IsArchived = true;
            author.ArchivedDate = DateOnly.FromDateTime(DateTime.UtcNow);
            author.ArchivedByUserId = performedByUserId;

            _context.SaveChanges();
            return true;
        }
    }
}
