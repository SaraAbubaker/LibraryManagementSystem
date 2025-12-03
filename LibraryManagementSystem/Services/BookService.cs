using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Extensions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class BookService
    {
        private readonly IGenericRepository<Book> _bookRepo;

        public BookService(IGenericRepository<Book> bookRepo)
        {
            _bookRepo = bookRepo;
        }

        //CRUD
        public async Task<Book> CreateBookAsync(CreateBookDto dto, int currentUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(currentUserId, "currentUserId");

            var book = dto.Adapt<Book>();  //Mapping using Mapster

            book.CreatedByUserId = currentUserId;
            book.CreatedDate = DateOnly.FromDateTime(DateTime.Now);

            await _bookRepo.AddAsync(book);

            return book;
        }

        public async Task<BookListDto?> GetBookDetailsAsync(int bookId)
        {
            var book = await _bookRepo.GetByIdAsync(bookId);
            if (book == null || book.IsArchived) return null;

            var dto = book.Adapt<BookListDto>();
            dto.AuthorName = book.Author?.Name ?? "Unknown";
            dto.CategoryName = book.Category?.Name ?? "Unknown";
            dto.PublisherName = book.Publisher?.Name ?? "Unknown";
            dto.IsAvailable = book.InventoryRecords?.Any(r => r.IsAvailable) ?? false;

            return dto;
        }

        public async Task<List<BookListDto>> GetBooksByAuthorAsync(int authorId)
        {
            var books = (await _bookRepo.GetAllAsync())
                .Where(b => b.AuthorId == authorId && !b.IsArchived)
                .ToList();

            return books.Select(b => new BookListDto
            {
                Id = b.Id,
                Title = b.Title,
                PublishDate = b.PublishDate,
                AuthorName = b.Author?.Name ?? "Unknown",
                CategoryName = b.Category?.Name ?? "Unknown",
                PublisherName = b.Publisher?.Name ?? "Unknown",
                IsAvailable = b.InventoryRecords?.Any(r => r.IsAvailable) ?? false
            })
            .OrderBy(b => b.AuthorName)
            .ToList();
        }

        public async Task<List<BookListDto>> GetBooksByCategoryAsync(int categoryId)
        {
            var booksAll = await _bookRepo.GetAllAsync();

            var anyBookWithCategory = booksAll.Any(b => b.CategoryId == categoryId && !b.IsArchived);
            if (!anyBookWithCategory)
                categoryId = -1; //Unknown

            var books = booksAll
                .Where(b => b.CategoryId == categoryId && !b.IsArchived)
                .ToList();

            return books
                .Select(b => new BookListDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    PublishDate = b.PublishDate,
                    AuthorName = b.Author?.Name ?? "Unknown",
                    CategoryName = b.Category?.Name ?? "Unknown",
                    PublisherName = b.Publisher?.Name ?? "Unknown",
                    IsAvailable = b.InventoryRecords?.Any(r => r.IsAvailable) ?? false
                })
                .ToList();
        }

        public async Task<bool> UpdateBookAsync(UpdateBookDto dto, int currentUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(dto.Id, "Id");
            Validate.Positive(currentUserId, "currentUserId");

            var book = await _bookRepo.GetByIdAsync(dto.Id);
            if (book == null)
                throw new NotFoundException($"Book with id {dto.Id} not found.");

            if (dto.Title != null) book.Title = dto.Title;
            if (dto.PublishDate != null) book.PublishDate = dto.PublishDate.Value;
            if (dto.Version != null) book.Version = dto.Version;

            if (dto.PublisherId.HasValue)
                book.PublisherId = dto.PublisherId.Value;

            if (dto.AuthorId.HasValue)
                book.AuthorId = dto.AuthorId.Value;

            if (dto.CategoryId.HasValue)
                book.CategoryId = dto.CategoryId.Value;

            book.LastModifiedByUserId = currentUserId;
            book.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _bookRepo.UpdateAsync(book);
            return true;
        }

        public async Task<bool> ArchiveBookAsync(int bookId, int performedByUserId)
        {
            Validate.Positive(bookId, "bookId");
            Validate.Positive(performedByUserId, "performedByUserId");

            var book = await _bookRepo.GetByIdAsync(bookId);
            if (book == null)
                throw new NotFoundException($"Book with id {bookId} not found.");

            book.IsArchived = true;
            book.ArchivedByUserId = performedByUserId;
            book.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            await _bookRepo.UpdateAsync(book);
            return true;
        }

        //Search method (filter, sort, pagination)
        public async Task<List<BookListDto>> SearchBooksAsync(SearchBookParamsDto dto)
        {
            Validate.NotNull(dto, nameof(dto));

            var skip = (Math.Max(1, dto.Page) - 1) * Math.Max(1, dto.PageSize);

            var booksAll = await _bookRepo.GetAllAsync();
            var books = booksAll.AsQueryable();

            //Search
            if (!string.IsNullOrWhiteSpace(dto.SearchParam))
            {
                var search = dto.SearchParam.Trim();
                books = books.Where(b =>
                    EF.Functions.Like(b.Title, $"%{search}%") ||
                    (b.Author != null && EF.Functions.Like(b.Author.Name, $"%{search}%")) ||
                    (b.Category != null && EF.Functions.Like(b.Category.Name, $"%{search}%")) ||
                    (b.Publisher != null && EF.Functions.Like(b.Publisher.Name, $"%{search}%"))
                );
            }

            //Filter
            if (!string.IsNullOrWhiteSpace(dto.Title))
                books = books.Where(b => b.Title != null && b.Title.Contains(dto.Title.Trim(), StringComparison.OrdinalIgnoreCase));

            if (dto.AuthorId.HasValue)
                books = books.Where(b => b.AuthorId == dto.AuthorId.Value);

            if (dto.CategoryId.HasValue)
                books = books.Where(b => b.CategoryId == dto.CategoryId.Value);

            if (dto.PublisherId.HasValue)
                books = books.Where(b => b.PublisherId == dto.PublisherId.Value);

            if (dto.IsAvailable.HasValue)
            {
                bool available = dto.IsAvailable.Value;
                books = books.Where(b => (b.InventoryRecords.Count(r => r.IsAvailable) > 0) == available);
            }


            if (dto.PublishDate.HasValue)
                books = books.Where(b => b.PublishDate == dto.PublishDate.Value);

            //Sorting
            books = (dto.SortBy?.Trim().ToLower()) switch
            {
                "title" => dto.SortDir?.ToLower() == "desc" ? books.OrderByDescending(b => b.Title) : books.OrderBy(b => b.Title),
                "publishdate" => dto.SortDir?.ToLower() == "desc" ? books.OrderByDescending(b => b.PublishDate) : books.OrderBy(b => b.PublishDate),
                "author" => dto.SortDir?.ToLower() == "desc" ? books.OrderByDescending(b => b.Author != null ? b.Author.Name : "") : books.OrderBy(b => b.Author != null ? b.Author.Name : ""),
                "category" => dto.SortDir?.ToLower() == "desc" ? books.OrderByDescending(b => b.Category != null ? b.Category.Name : "") : books.OrderBy(b => b.Category != null ? b.Category.Name : ""),
                "publisher" => dto.SortDir?.ToLower() == "desc" ? books.OrderByDescending(b => b.Publisher != null ? b.Publisher.Name : "") : books.OrderBy(b => b.Publisher != null ? b.Publisher.Name : ""),
                "isavailable" => dto.SortDir?.ToLower() == "desc"
                ? books.OrderByDescending(b => b.InventoryRecords.Count(r => r.IsAvailable) > 0)
                : books.OrderBy(b => b.InventoryRecords.Count(r => r.IsAvailable) > 0),
                _ => books.OrderBy(b => b.Title)
            };

            //Pagination + mapping
            var paged = books.Skip(skip).Take(Math.Max(1, dto.PageSize)).ToList();

            return paged.Select(b => new BookListDto
            {
                Id = b.Id,
                Title = b.Title,
                PublishDate = b.PublishDate,
                AuthorName = b.Author?.Name ?? "Unknown",
                CategoryName = b.Category?.Name ?? "Unknown",
                PublisherName = b.Publisher?.Name ?? "Unknown",
                IsAvailable = b.InventoryRecords?.Any(r => r.IsAvailable) ?? false
            }).ToList();
        }
    }
}
