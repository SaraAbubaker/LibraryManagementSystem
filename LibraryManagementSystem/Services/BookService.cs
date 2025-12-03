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
        public Book CreateBook(CreateBookDto dto, int currentUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(currentUserId, "currentUserId");

            var book = dto.Adapt<Book>();  //Mapping using Mapster

            book.CreatedByUserId = currentUserId;
            book.CreatedDate = DateOnly.FromDateTime(DateTime.Now);

            _bookRepo.Add(book);

            return book;
        }

        public BookListDto? GetBookDetails(int bookId)
        {
            var book = _bookRepo.GetById(bookId);

            var dto = book.Adapt<BookListDto>();
            dto.AuthorName = book.Author?.Name ?? "Unknown";
            dto.CategoryName = book.Category?.Name ?? "Unknown";
            dto.PublisherName = book.Publisher?.Name ?? "Unknown";
            dto.IsAvailable = book.InventoryRecords?.Any(r => r.IsAvailable) ?? false;

            return dto;
        }

        public List<BookListDto> GetBooksByAuthor(int authorId)
        {
            var books = _bookRepo.GetAll()
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

        public List<BookListDto> GetBooksByCategory(int categoryId)
        {
            var anyBookWithCategory = _bookRepo.GetAll().Any(b => b.CategoryId == categoryId && !b.IsArchived);
            if (!anyBookWithCategory)
                categoryId = -1; // unknown

            var books = _bookRepo.GetAll()
                .Where(b => b.CategoryId == categoryId && !b.IsArchived)
                .ToList();

            var dtos = books.Select(b => new BookListDto
            {
                Id = b.Id,
                Title = b.Title,
                PublishDate = b.PublishDate,
                AuthorName = b.Author?.Name ?? "Unknown",
                CategoryName = b.Category?.Name ?? "Unknown",
                PublisherName = b.Publisher?.Name ?? "Unknown",
                IsAvailable = b.InventoryRecords?.Any(r => r.IsAvailable) ?? false
            }).ToList();

            return dtos;
        }

        public bool UpdateBook(UpdateBookDto dto, int currentUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(dto.Id, "Id");
            Validate.Positive(currentUserId, "currentUserId");

            var book = _bookRepo.GetById(dto.Id);
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

            _bookRepo.Update(book);
            return true;
        }

        public bool ArchiveBook(int bookId, int performedByUserId)
        {
            Validate.Positive(bookId, "bookId");
            Validate.Positive(performedByUserId, "performedByUserId");

            var book = _bookRepo.GetById(bookId);
            if (book == null)
                throw new NotFoundException($"Book with id {bookId} not found.");

            book.IsArchived = true;
            book.ArchivedByUserId = performedByUserId;
            book.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            _bookRepo.Update(book);
            return true;
        }

        //Search method (filter, sort, pagination)
        public List<BookListDto> SearchBooks(SearchBookParamsDto dto)
        {
            Validate.NotNull(dto, nameof(dto));

            var skip = (Math.Max(1, dto.Page) - 1) * Math.Max(1, dto.PageSize);

            var books = _bookRepo.GetAll().AsQueryable();

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
