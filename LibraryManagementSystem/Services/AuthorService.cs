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
        public AuthorListDto CreateAuthor(CreateAuthorDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, "Author name");

            var author = new Author
            {
                Name = dto.Name,
                Email = dto.Email
            };

            _authorRepo.Add(author);

            var result = author.Adapt<AuthorListDto>();

            result.BookCount = _bookRepo.GetAll().Count(b => b.AuthorId == author.Id && !b.IsArchived);

            return result;
        }

        public List<AuthorListDto> ListAuthors()
        {
            var bookCounts = _bookRepo.GetAll()
                .Where(b => !b.IsArchived)
                .GroupBy(b => b.AuthorId)
                .ToDictionary(g => g.Key, g => g.Count());

            var authors = _authorRepo.GetAll()
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

        public AuthorListDto? GetAuthorById(int id)
        {
            Validate.Positive(id, "Id");

            var author = _authorRepo.GetById(id);
            if (author == null || author.IsArchived) return null;

            var dto = author.Adapt<AuthorListDto>();
            dto.BookCount = _bookRepo.GetAll().Count(b => b.AuthorId == author.Id && !b.IsArchived);

            return dto;
        }

        public bool EditAuthor(UpdateAuthorDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(dto.Id, "Id");
            Validate.NotEmpty(dto.Name, "Author name");

            var author = _authorRepo.GetById(dto.Id);
            if (author.IsArchived) throw new ConflictException($"Author with id {dto.Id} is archived.");

            author.Name = dto.Name;
            author.Email = dto.Email;

            _authorRepo.Update(author);

            return true;
        }

        public bool ArchiveAuthor(int id, int performedByUserId)
        {
            Validate.Positive(id, "Id");
            Validate.Positive(performedByUserId, "performedByUserId");

            var author = _authorRepo.GetById(id);

            if (author!.IsArchived)
                throw new ConflictException($"Author with id {id} is already archived.");

            var books = _bookRepo.GetAll().Where(b => b.AuthorId == id).ToList();
            foreach (var book in books)
            {
                book.AuthorId = -1; //Unknown
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
