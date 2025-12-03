using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Extensions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
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
        private readonly LibraryContext _context;
        public BookService(LibraryContext context)
        {
            _context = context;
        }

        //CRUD
        public Book CreateBook(CreateBookDto dto, int currentUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(currentUserId, "currentUserId");

            var book = dto.Adapt<Book>();  //Mapping using Mapster

            book.CreatedByUserId = currentUserId;
            book.CreatedDate = DateOnly.FromDateTime(DateTime.Now);

            _context.Books.Add(book);
            _context.SaveChanges();

            return book;
        }

        public BookListDto? GetBookDetails(int bookId)
        {
            var book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.InventoryRecords)
                .FirstOrDefault(b => b.Id == bookId);
            Validate.Exists(book, $"Book with id {bookId}");

            var dto = book.Adapt<BookListDto>();

            dto.AuthorName = book!.Author?.Name ?? "Unknown";
            dto.CategoryName = book!.Category?.Name ?? "Unknown";
            dto.PublisherName = book!.Publisher?.Name ?? "Unknown";
            dto.IsAvailable = book.InventoryRecords.Any(r => r.IsAvailable);

            return dto;
        }

        public List<BookListDto> GetBooksByAuthor(int authorId)
        {
            var author = _context.Authors
                .FirstOrDefault(a => a.Id == authorId && !a.IsArchived);
            Validate.Exists(author, "Author not found.");

            var query =
                _context.Books
                    .Where(b => b.AuthorId == authorId && !b.IsArchived)
                    .Include(b => b.Author)
                    .Include(b => b.Category)
                    .Include(b => b.Publisher)
                    .Include(b => b.InventoryRecords)
                    .AsEnumerable()
                    .Select(b => new BookListDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        PublishDate = b.PublishDate,
                        AuthorName = b.Author?.Name ?? "Unknown",
                        CategoryName = b.Category?.Name ?? "Unknown",
                        PublisherName = b.Publisher?.Name ?? "Unknown",
                        IsAvailable = b.InventoryRecords.Any(r => r.IsAvailable)
                    });

            //Sort by Author Name
            return query.OrderBy(x => x.AuthorName).ToList();
        }

        public List<BookListDto> GetBooksByCategory(int categoryId)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (category == null || category.IsArchived)
                categoryId = -1; // unknown

            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.InventoryRecords)
                .Where(b => b.CategoryId == categoryId)
                .ToList();

            return books.Select(b => new BookListDto
            {
                Id = b.Id,
                Title = b.Title,
                PublishDate = b.PublishDate,
                AuthorName = b.Author?.Name ?? "Unknown",
                CategoryName = b.Category?.Name ?? "Unknown",
                PublisherName = b.Publisher?.Name ?? "Unknown",
                IsAvailable = b.InventoryRecords.Any(r => r.IsAvailable)
            }).ToList();
        }

        public bool UpdateBook(UpdateBookDto dto, int currentUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(dto.Id, "Id");
            Validate.Positive(currentUserId, "currentUserId");

            var book = _context.Books
                .Include(b => b.Publisher)
                .FirstOrDefault(b => b.Id == dto.Id);
            if (book == null)
                throw new NotFoundException($"Book with id {dto.Id} not found.");

            if (dto.Title != null) book.Title = dto.Title;
            if (dto.PublishDate != null) book.PublishDate = dto.PublishDate.Value;
            if (dto.Version != null) book.Version = dto.Version;

            if (dto.PublisherId.HasValue)
            {
                var publisherExists = _context.Publishers.Any(p => p.Id == dto.PublisherId.Value && !p.IsArchived);
                if (!publisherExists) throw new NotFoundException("Publisher not found.");
                book.PublisherId = dto.PublisherId.Value;
            }

            if (dto.AuthorId.HasValue)
            {
                var authorExists = _context.Authors.Any(a => a.Id == dto.AuthorId.Value && !a.IsArchived);
                if (!authorExists) throw new NotFoundException("Author not found.");
                book.AuthorId = dto.AuthorId.Value;
            }

            if (dto.CategoryId.HasValue)
            {
                var categoryExists = _context.Categories.Any(c => c.Id == dto.CategoryId.Value && !c.IsArchived);
                if (!categoryExists) throw new NotFoundException("Category not found.");
                book.CategoryId = dto.CategoryId.Value;
            }

            book.LastModifiedByUserId = currentUserId;
            book.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            _context.SaveChanges();
            return true;
        }

        public bool ArchiveBook(int bookId, int performedByUserId)
        {
            Validate.Positive(bookId, "bookId");
            Validate.Positive(performedByUserId, "performedByUserId");

            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
                throw new NotFoundException($"Book with id {bookId} not found.");

            book.IsArchived = true;
            book.ArchivedByUserId = performedByUserId;
            book.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            _context.SaveChanges();
            return true;
        }

        //Search method (filter, sort, pagination)
        public List<BookListDto> SearchBooks(SearchBookParamsDto dto)
        {
            Validate.NotNull(dto, nameof(dto));

            var skip = (Math.Max(1, dto.Page) - 1) * Math.Max(1, dto.PageSize);

            var query = _context.Books
                .Include(b => b.InventoryRecords)
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .AsQueryable();

            //Search
            if (!string.IsNullOrWhiteSpace(dto.SearchParam))
            {
                var search = dto.SearchParam.Trim();
                query = query.Where(b =>
                    EF.Functions.Like(b.Title, $"%{search}%") ||
                    (b.Author != null && EF.Functions.Like(b.Author.Name, $"%{search}%")) ||
                    (b.Category != null && EF.Functions.Like(b.Category.Name, $"%{search}%")) ||
                    (b.Publisher != null && EF.Functions.Like(b.Publisher.Name, $"%{search}%"))
                );
            }

            //Filter
            if (!string.IsNullOrWhiteSpace(dto.Title))
            {
                var titleFilter = dto.Title.Trim();
                query = query.Where(b => EF.Functions.Like(b.Title, $"%{titleFilter}%"));
            }

            if (dto.AuthorId.HasValue)
                query = query.Where(b => b.AuthorId == dto.AuthorId.Value);

            if (dto.CategoryId.HasValue)
                query = query.Where(b => b.CategoryId == dto.CategoryId.Value);

            if (dto.PublisherId.HasValue)
                query = query.Where(b => b.PublisherId == dto.PublisherId.Value);

            if (dto.IsAvailable.HasValue)
            {
                query = query.Where(b =>
                    dto.IsAvailable.Value ? b.InventoryRecords.Any(r => r.IsAvailable)
                                          : !b.InventoryRecords.Any(r => r.IsAvailable)
                );
            }

            if (dto.PublishDate.HasValue)
                query = query.Where(b => b.PublishDate == dto.PublishDate.Value);

            //Sorting
            query = (dto.SortBy?.Trim().ToLower()) switch
            {
                "title" => dto.SortDir?.ToLower() == "desc" ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
                "publishdate" => dto.SortDir?.ToLower() == "desc" ? query.OrderByDescending(b => b.PublishDate) : query.OrderBy(b => b.PublishDate),
                "author" => dto.SortDir?.ToLower() == "desc" ? query.OrderByDescending(b => b.Author != null ? b.Author.Name : "") : query.OrderBy(b => b.Author != null ? b.Author.Name : ""),
                "category" => dto.SortDir?.ToLower() == "desc" ? query.OrderByDescending(b => b.Category != null ? b.Category.Name : "") : query.OrderBy(b => b.Category != null ? b.Category.Name : ""),
                "publisher" => dto.SortDir?.ToLower() == "desc" ? query.OrderByDescending(b => b.Publisher != null ? b.Publisher.Name : "") : query.OrderBy(b => b.Publisher != null ? b.Publisher.Name : ""),
                "isavailable" => dto.SortDir?.ToLower() == "desc"
                ? query.OrderByDescending(b => b.InventoryRecords.Any(r => r.IsAvailable))
                : query.OrderBy(b => b.InventoryRecords.Any(r => r.IsAvailable)),
                _ => query.OrderBy(b => b.Title)
            };

            //Pagination + mapping
            var books = query
                .Skip(skip)
                .Take(Math.Max(1, dto.PageSize))
                .ToList();

            return books.Select(b => new BookListDto
            {
                Id = b.Id,
                Title = b.Title,
                PublishDate = b.PublishDate,
                AuthorName = b.Author?.Name ?? "Unknown",
                CategoryName = b.Category?.Name ?? "Unknown",
                PublisherName = b.Publisher?.Name ?? "Unknown",
                IsAvailable = b.InventoryRecords.Any(r => r.IsAvailable)
            }).ToList();
        }
    }
}
