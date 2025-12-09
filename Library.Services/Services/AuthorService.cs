using Library.Domain.Repositories;
using Library.Entities.Models;
using Library.Services.Interfaces;
using Library.Shared.DTOs.Author;
using Library.Shared.Exceptions;
using Library.Shared.Helpers;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Library.Services.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IGenericRepository<Author> _authorRepo;
        private readonly IGenericRepository<Book> _bookRepo;

        public AuthorService(IGenericRepository<Author> authorRepo, IGenericRepository<Book> bookRepo)
        {
            _authorRepo = authorRepo;
            _bookRepo = bookRepo;
        }

        //CRUD
        public async Task<AuthorListDto> CreateAuthorAsync(CreateAuthorDto dto, int userId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Name, nameof(dto.Name));
            Validate.Positive(userId, nameof(userId));

            var name = dto.Name.Trim();
            var email = dto.Email?.Trim();

            var existingAuthors = _authorRepo.GetAll();
            if (existingAuthors.Any(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"An author with the name '{name}' already exists.");

            var author = new Author
            {
                Name = dto.Name,
                Email = dto.Email,
                IsArchived = false,
                CreatedByUserId = userId,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                LastModifiedByUserId = userId,
                LastModifiedDate = DateOnly.FromDateTime(DateTime.Now),
            };

            await _authorRepo.AddAsync(author);

            var result = author.Adapt<AuthorListDto>();

            var books = _bookRepo.GetAll();
            result.BookCount = books.Count(b => b.AuthorId == author.Id);

            return result;
        }

        public IQueryable<AuthorListDto> ListAuthorsQuery()
        {
            return _authorRepo.GetAll()
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .Select(a => new AuthorListDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    BookCount = a.Books.Count()
                });
        }

        public IQueryable<AuthorListDto> GetAuthorByIdQuery(int id)
        {
            Validate.Positive(id, nameof(id));

            return _authorRepo.GetAll()
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AuthorListDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Email = a.Email,

                    BookCount = _bookRepo.GetAll()
                        .Count(b => b.AuthorId == a.Id)
                });
        }

        public async Task<bool> EditAuthorAsync(UpdateAuthorDto dto, int userId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(dto.Id, nameof(dto.Id));

            var name = (dto.Name ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(name))
                throw new ValidationException("Name is required.");

            var author = await _authorRepo.GetById(dto.Id).FirstOrDefaultAsync();

            if (author == null)
                throw new InvalidOperationException($"Author with id {dto.Id} not found.");

            var nameLower = name.ToLower();
            var exists = await _authorRepo.GetAll()
                .Where(a => a.Id != dto.Id)
                .AnyAsync(a => a.Name.ToLower() == nameLower);

            if (exists)
                throw new InvalidOperationException($"Another author with the name '{name}' already exists.");

            author.Name = name;
            author.Email = dto.Email;
            author.LastModifiedByUserId = userId;
            author.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _authorRepo.UpdateAsync(author);

            return true;
        }

        public async Task<bool> ArchiveAuthorAsync(int id, int performedByUserId)
        {
            Validate.Positive(id, nameof(id));
            Validate.Positive(performedByUserId, nameof(performedByUserId));

            var author = await _authorRepo.GetById(id).FirstOrDefaultAsync();
            if (author == null) throw new NotFoundException($"Author with id {id} not found.");

            var books = (_bookRepo.GetAll()).Where(b => b.AuthorId == id);
            foreach (var book in books)
            {
                book.AuthorId = -1; // Unknown
                await _bookRepo.UpdateAsync(book);
            }

            author.Name = "Unknown";
            author.Email = string.Empty;
            author.IsArchived = true;
            author.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);
            author.ArchivedByUserId = performedByUserId;
            author.LastModifiedByUserId = performedByUserId;
            author.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _authorRepo.UpdateAsync(author);
            return true;
        }
    }
}
