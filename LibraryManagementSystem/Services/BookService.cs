using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Extensions;
using LibraryManagementSystem.Helpers;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Services
{
    public class BookService
    {
        private readonly LibraryDataStore Store;
        public BookService(LibraryDataStore store)
        {
            Store = store;
        }

        //CRUD
        public Book CreateBook(CreateBookDto dto, int currentUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(currentUserId, "currentUserId");

            var book = dto.Adapt<Book>();  //Mapping using Mapster

            book.Id = (Store.Books.Select(b => b.Id).DefaultIfEmpty(0).Max()) + 1;

            book.CreatedByUserId = currentUserId;
            book.CreatedDate = DateOnly.FromDateTime(DateTime.Now);


            Store.Books.Add(book);
            return book;
        }

        public BookListDto? GetBookDetails(int bookId)
        {
            var book = Store.Books.FirstOrDefault(b => b.Id == bookId);
            Validate.Exists(book, $"Book with id {bookId}");

            var dto = book.Adapt<BookListDto>();

            var result =
                (from b in Store.Books
                 where b.Id == bookId
                 join a in Store.Authors on b.AuthorId equals a.Id into ag
                 from author in ag.DefaultIfEmpty()
                 join c in Store.Categories on b.CategoryId equals c.Id into cg
                 from category in cg.DefaultIfEmpty()
                 select new
                 {
                     AuthorName = author?.Name ?? "Unknown",
                     CategoryName = category?.Name ?? "Unknown",
                     IsAvailable = Store.InventoryRecords.Any(r => r.BookId == b.Id && r.IsAvailable)
                 }).First();

            dto.AuthorName = result.AuthorName;
            dto.CategoryName = result.CategoryName;
            dto.IsAvailable = result.IsAvailable;

            return dto;
        }

        public List<BookListDto> GetBooksByAuthor(int authorId)
        {
            var author = Store.Authors
                .FirstOrDefault(a => a.Id == authorId && !a.IsArchived);
            Validate.Exists(author, "Author not found.");

            var query =
                from b in Store.Books
                where b.AuthorId == authorId && !b.IsArchived
                join a in Store.Authors.Where(x => !x.IsArchived) on b.AuthorId equals a.Id into ag
                from auth in ag.DefaultIfEmpty()
                join c in Store.Categories.Where(x => !x.IsArchived) on b.CategoryId equals c.Id into cg
                from cat in cg.DefaultIfEmpty()
                join r in Store.InventoryRecords on b.Id equals r.BookId into invGroup
                from inv in invGroup.DefaultIfEmpty()

                group new { b, auth, cat, inv } by b into g
                select new BookListDto
                {
                    Id = g.Key.Id,
                    Title = g.Key.Title,
                    PublishDate = g.Key.PublishDate,
                    AuthorName = g.Select(x => x.auth?.Name).FirstOrDefault() ?? "Unknown",
                    CategoryName = g.Select(x => x.cat?.Name).FirstOrDefault() ?? "Unknown",
                    IsAvailable = g.Any(x => x.inv != null && x.inv.IsAvailable)
                };

            //Sort by Author Name
            return query.OrderBy(x => x.AuthorName).ToList();
        }

        public List<BookListDto> GetBooksByCategory(int categoryId)
        {
            var category = Store.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (category == null || category.IsArchived)
                categoryId = 0; // unknown

            var books = Store.Books
                .Where(b => b.CategoryId == categoryId)
                .ToList();

            return books.Adapt<List<BookListDto>>();
        }

        public bool UpdateBook(UpdateBookDto dto, int currentUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(dto.Id, "Id");
            Validate.Positive(currentUserId, "currentUserId");

            if (Store.Books.FirstOrDefault(b => b.Id == dto.Id) is not Book book)
                throw new LibraryManagementSystem.Exceptions.NotFoundException($"Book with id {dto.Id} not found.");

            if (dto.Title != null) book.Title = dto.Title;
            if (dto.PublishDate != null) book.PublishDate = dto.PublishDate.Value;
            if (dto.Version != null) book.Version = dto.Version;
            if (dto.Publisher != null) book.Publisher = dto.Publisher;
            if (dto.AuthorId != null) book.AuthorId = dto.AuthorId.Value;
            if (dto.CategoryId != null) book.CategoryId = dto.CategoryId.Value;

            book.LastModifiedByUserId = currentUserId;
            book.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            return true;
        }

        public bool ArchiveBook(int bookId, int performedByUserId)
        {
            Validate.Positive(bookId, "bookId");
            Validate.Positive(performedByUserId, "performedByUserId");

            if (Store.Books.FirstOrDefault(b => b.Id == bookId) is not Book book)
                throw new LibraryManagementSystem.Exceptions.NotFoundException($"Book with id {bookId} not found.");

            book.IsArchived = true;
            book.ArchivedByUserId = performedByUserId;
            book.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            return true;
        }

        //Search method (filter, sort, pagination)
        public List<BookListDto> SearchBooks(SearchBookParamsDto dto)
        {
            Validate.NotNull(dto, nameof(dto));

            var authorsById = Store.Authors.ToDictionary(a => a.Id, a => a.Name);
            var categoriesById = Store.Categories.ToDictionary(c => c.Id, c => c.Name);
            var availableBookIds = new HashSet<int>(
                Store.InventoryRecords.Where(r => r.IsAvailable).Select(r => r.BookId)
            );

            var skip = (Math.Max(1, dto.Page) - 1) * Math.Max(1, dto.PageSize);

            var results = Store.Books
                //Search
                .WhereIf(!string.IsNullOrWhiteSpace(dto.SearchParam), b =>
                {
                    string authorName = authorsById.TryGetValue(b.AuthorId, out var an) ? an : "";
                    string categoryName = categoriesById.TryGetValue(b.CategoryId, out var cn) ? cn : "";
                    var search = dto.SearchParam!;
                    return (b.Title ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                           || authorName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                           || categoryName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                           || (b.Publisher ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                })

                //Filter
                .WhereIf(!string.IsNullOrWhiteSpace(dto.Title), b =>
                    (b.Title ?? "").IndexOf(dto.Title!, StringComparison.OrdinalIgnoreCase) >= 0)
                .WhereIf(dto.AuthorId.HasValue, b => b.AuthorId == dto.AuthorId!.Value)
                .WhereIf(dto.CategoryId.HasValue, b => b.CategoryId == dto.CategoryId!.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(dto.Publisher), b =>
                    (b.Publisher ?? "").IndexOf(dto.Publisher!, StringComparison.OrdinalIgnoreCase) >= 0)
                .WhereIf(dto.IsAvailable.HasValue, b =>
                    availableBookIds.Contains(b.Id) == dto.IsAvailable!.Value)
                .WhereIf(dto.PublishDate.HasValue, b => b.PublishDate == dto.PublishDate!.Value);

            //Sort
            var sortKey = (dto.SortBy ?? "title").Trim().ToLower();
            var desc = dto.SortDir?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

            var keySelector = new Dictionary<string, Func<Book, object>>(StringComparer.OrdinalIgnoreCase)
            {
                ["title"] = b => b.Title ?? "",
                ["publishdate"] = b => b.PublishDate,
                ["author"] = b => authorsById.TryGetValue(b.AuthorId, out var an) ? an : "",
                ["category"] = b => categoriesById.TryGetValue(b.CategoryId, out var cn) ? cn : "",
                ["publisher"] = b => b.Publisher ?? "",
                ["isavailable"] = b => availableBookIds.Contains(b.Id)
            };

            var selector = keySelector.TryGetValue(sortKey, out var sel) ? sel : keySelector["title"];
            var ordered = desc ? results.OrderByDescending(selector) : results.OrderBy(selector);

            //Pagination + mapping + DTO
            return ordered
                .Skip(skip)
                .Take(Math.Max(1, dto.PageSize))
                .Select(b =>
                {
                    var outDto = b.Adapt<BookListDto>();
                    outDto.AuthorName = authorsById.TryGetValue(b.AuthorId, out var an) ? an : "Unknown";
                    outDto.CategoryName = categoriesById.TryGetValue(b.CategoryId, out var cn) ? cn : "Unknown";
                    outDto.Publisher = b.Publisher ?? string.Empty;
                    outDto.IsAvailable = availableBookIds.Contains(b.Id);
                    return outDto;
                })
                .ToList();
        }
    }
}
