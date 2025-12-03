using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs.Author;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryManagementSystem.Services
{
    public class AuthorService
    {
        private readonly IGenericRepository<Author> _authorRepo;
        private readonly IGenericRepository<Book> _bookRepo;

        public AuthorService(IGenericRepository<Author> authorRepo, IGenericRepository<Book> bookRepo)
        {
            _authorRepo = authorRepo;
            _bookRepo = bookRepo;
        }

        //CRUD
        public async Task<AuthorListDto> CreateAuthorAsync(CreateAuthorDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Author name");

            var author = new Author
            {
                Name = dto.Name,
                Email = dto.Email
            };

            await _authorRepo.AddAsync(author);

            var result = author.Adapt<AuthorListDto>();

            var books = await _bookRepo.GetAllAsync();
            result.BookCount = books.Count(b => b.AuthorId == author.Id && !b.IsArchived);

            return result;
        }

        public async Task<List<AuthorListDto>> ListAuthorsAsync()
        {
            var books = await _bookRepo.GetAllAsync();
            var bookCounts = books
                .Where(b => !b.IsArchived)
                .GroupBy(b => b.AuthorId)
                .ToDictionary(g => g.Key, g => g.Count());


            var authors = await _authorRepo.GetAllAsync();
            var activeAuthors = authors.Where(a => !a.IsArchived).OrderBy(a => a.Name);

            return activeAuthors
                .Select(a =>
                {
                    var dto = a.Adapt<AuthorListDto>();
                    dto.BookCount = bookCounts.TryGetValue(a.Id, out var count) ? count : 0;
                    return dto;
                })
                .ToList();
        }

        public async Task<AuthorListDto?> GetAuthorByIdAsync(int id)
        {
            Validate.Positive(id, "Id");

            var author = await _authorRepo.GetByIdAsync(id);
            if (author == null || author.IsArchived) return null;

            var dto = author.Adapt<AuthorListDto>();
            var books = await _bookRepo.GetAllAsync();
            dto.BookCount = books.Count(b => b.AuthorId == author.Id && !b.IsArchived);

            return dto;
        }

        public async Task<bool> EditAuthorAsync(UpdateAuthorDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(dto.Id, "Id");
            Validate.NotEmpty(dto.Name, "Author name");

            var author = await _authorRepo.GetByIdAsync(dto.Id);
            if (author.IsArchived) throw new ConflictException($"Author with id {dto.Id} is archived.");

            author.Name = dto.Name;
            author.Email = dto.Email;

            _authorRepo.Update(author);

            return true;
        }

        public async Task<bool> ArchiveAuthorAsync(int id, int performedByUserId)
        {
            Validate.Positive(id, "Id");
            Validate.Positive(performedByUserId, "performedByUserId");

            var author = await _authorRepo.GetByIdAsync(id);
            if (author == null) throw new NotFoundException($"Author with id {id} not found.");
            if (author.IsArchived) throw new ConflictException($"Author with id {id} is already archived.");

            var books = (await _bookRepo.GetAllAsync()).Where(b => b.AuthorId == id).ToList();
            foreach (var book in books)
            {
                book.AuthorId = -1; // Unknown
                _bookRepo.Update(book);
            }

            author.Name = "Unknown";
            author.Email = string.Empty;
            author.IsArchived = true;
            author.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);
            author.ArchivedByUserId = performedByUserId;

            _authorRepo.Update(author);
            return true;
        }
    }
}
