using Library.Domain.Repositories;
using Library.Shared.Helpers;
using Library.Shared.DTOs.Book;
using Library.Shared.Exceptions;
using Library.Entities.Models;
using Library.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace Library.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IGenericRepository<Book> _bookRepo;
        private readonly IGenericRepository<InventoryRecord> _inventoryRepo;

        public BookService(IGenericRepository<Book> bookRepo, 
            IGenericRepository<InventoryRecord> inventoryRepo)
        {
            _bookRepo = bookRepo;
            _inventoryRepo = inventoryRepo;
        }

        //CRUD
        public async Task<Book> CreateBookAsync(CreateBookDto dto, int currentUserId)
        {
            Validate.ValidateModel(dto);
            Validate.Positive(currentUserId, nameof(currentUserId));

            var book = dto.Adapt<Book>();  //Mapping using Mapster

            book.CreatedByUserId = currentUserId;
            book.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            book.LastModifiedByUserId = currentUserId;
            book.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            book.IsArchived = false;

            await _bookRepo.AddAsync(book);

            return book;
        }

        public IQueryable<BookListDto?> GetBookDetailsQuery(int bookId)
        {
            Validate.Positive(bookId, nameof(bookId));

            return _bookRepo.GetAll()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.InventoryRecords)
                .AsNoTracking()
                .Where(b => b.Id == bookId)
                .Select(b => new BookListDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    PublishDate = b.PublishDate,
                    AuthorName = b.Author != null ? b.Author.Name : "Unknown",
                    CategoryName = b.Category != null ? b.Category.Name : "Unknown",
                    PublisherName = b.Publisher != null ? b.Publisher.Name : "Unknown",
                    IsAvailable = b.InventoryRecords.Any(r => r.IsAvailable)
                });
        }

        public IQueryable<BookListDto> GetBooksByAuthorQuery(int authorId)
        {
            Validate.Positive(authorId, nameof(authorId));

            return _bookRepo.GetAll()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.InventoryRecords)
                .AsNoTracking()
                .Where(b => b.AuthorId == authorId)
                .Select(b => new BookListDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    PublishDate = b.PublishDate,
                    AuthorName = b.Author != null ? b.Author.Name : "Unknown",
                    CategoryName = b.Category != null ? b.Category.Name : "Unknown",
                    PublisherName = b.Publisher != null ? b.Publisher.Name : "Unknown",
                    IsAvailable = b.InventoryRecords.Any(r => r.IsAvailable)
                })
                .OrderBy(b => b.AuthorName);
        }

        public IQueryable<BookListDto> GetBooksByCategoryQuery(int categoryId)
        {
            Validate.Positive(categoryId, nameof(categoryId));

            var books = _bookRepo.GetAll().AsNoTracking();

            return _bookRepo.GetAll()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.InventoryRecords)
                .AsNoTracking()
                .Where(b => b.CategoryId == categoryId)
                .Select(b => new BookListDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    PublishDate = b.PublishDate,
                    AuthorName = b.Author != null ? b.Author.Name : "Unknown",
                    CategoryName = b.Category != null ? b.Category.Name : "Unknown",
                    PublisherName = b.Publisher != null ? b.Publisher.Name : "Unknown",
                    IsAvailable = b.InventoryRecords.Any(r => r.IsAvailable)
                });
        }

        public async Task<bool> UpdateBookAsync(UpdateBookDto dto, int currentUserId)
        {
            Validate.ValidateModel(dto);
            Validate.Positive(currentUserId, nameof(currentUserId));

            var book = Validate.Exists(
                await _bookRepo.GetAll()
                    .FirstOrDefaultAsync(b => b.Id == dto.Id),
                dto.Id
            );

            if (!string.IsNullOrWhiteSpace(dto.Title)) book.Title = dto.Title;
            if (dto.PublishDate.HasValue) book.PublishDate = dto.PublishDate.Value;
            if (dto.Version != null) book.Version = dto.Version;

            if (dto.PublisherId.HasValue) book.PublisherId = dto.PublisherId.Value;
            if (dto.AuthorId.HasValue) book.AuthorId = dto.AuthorId.Value;
            if (dto.CategoryId.HasValue) book.CategoryId = dto.CategoryId.Value;

            book.LastModifiedByUserId = currentUserId;
            book.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _bookRepo.UpdateAsync(book);
            return true;
        }

        public async Task<bool> ArchiveBookAsync(int bookId, int performedByUserId)
        {
            Validate.Positive(bookId, nameof(bookId));
            Validate.Positive(performedByUserId, nameof(performedByUserId));

            var book = Validate.Exists(
                await _bookRepo.GetAll()
                    .FirstOrDefaultAsync(b => b.Id == bookId),
                bookId
            );

            book.IsArchived = true;
            book.ArchivedByUserId = performedByUserId;
            book.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);
            book.LastModifiedByUserId = performedByUserId;
            book.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _bookRepo.UpdateAsync(book);
            return true;
        }

        //Search method (filter, sort, pagination)
        public IQueryable<BookListDto> SearchBooksQuery(SearchBookParamsDto dto)
        {
            Validate.ValidateModel(dto);

            var skip = (Math.Max(1, dto.Page) - 1) * Math.Max(1, dto.PageSize);

            var books = _bookRepo.GetAll()
                .Include(b => b.Author) 
                .Include(b => b.Category) 
                .Include(b => b.Publisher) 
                .Include(b => b.InventoryRecords)
                .AsNoTracking();

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
            var paged = books.Skip(skip).Take(Math.Max(1, dto.PageSize));

            return paged.Select(b => new BookListDto
            {
                Id = b.Id,
                Title = b.Title,
                PublishDate = b.PublishDate,
                AuthorName = b.Author != null ? b.Author.Name : "Unknown",
                CategoryName = b.Category != null ? b.Category.Name : "Unknown",
                PublisherName = b.Publisher != null ? b.Publisher.Name : "Unknown",
                IsAvailable = b.InventoryRecords.Any(r => r.IsAvailable)
            });
        }
    }
}
