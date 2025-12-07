using Library.Domain.Repositories;
using Library.Shared.Helpers;
using Library.Shared.DTOs.Author;
using Library.Shared.Exceptions;
using Library.Entities.Models;
using Library.Services.Interfaces;
using Mapster;

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

            var existingAuthors = (await _authorRepo.GetAllAsync()).ToList();
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
            Validate.Positive(id, nameof(id));

            var author = await _authorRepo.GetByIdAsync(id);
            if (author == null || author.IsArchived) return null;

            var dto = author.Adapt<AuthorListDto>();
            var books = await _bookRepo.GetAllAsync();
            dto.BookCount = books.Count(b => b.AuthorId == author.Id && !b.IsArchived);

            return dto;
        }

        public async Task<bool> EditAuthorAsync(UpdateAuthorDto dto, int userId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(dto.Id, nameof(dto.Id));
            Validate.NotEmpty(dto.Name, nameof(dto.Name));
            Validate.Positive(userId, nameof(userId));

            var author = await _authorRepo.GetByIdAsync(dto.Id);
            if (author.IsArchived) throw new ConflictException($"Author with id {dto.Id} is archived.");

            var name = dto.Name.Trim();
            var email = dto.Email?.Trim();

            var authors = await _authorRepo.GetAllAsync();
            if (authors.Any(a =>
                a.Id != dto.Id &&
                string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Another author with the name '{name}' already exists.");
            }

            author.Name = dto.Name;
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

            var author = await _authorRepo.GetByIdAsync(id);
            if (author == null) throw new NotFoundException($"Author with id {id} not found.");
            if (author.IsArchived) throw new ConflictException($"Author with id {id} is already archived.");

            var books = (await _bookRepo.GetAllAsync()).Where(b => b.AuthorId == id).ToList();
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
