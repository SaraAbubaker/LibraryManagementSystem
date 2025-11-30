using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Extensions;
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
            var book = dto.Adapt<Book>();  //Mapping using Mapster

            book.Id = (Store.Books.Select(b => b.Id).DefaultIfEmpty(0).Max()) + 1;

            book.CreatedByUserId = currentUserId;
            book.CreatedDate = DateTime.Now;

            Store.Books.Add(book);
            return book;
        }

        public BookListDto? GetBookDetails(int bookId)
        {
            var book = Store.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return null;

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
                .FirstOrDefault(a => a.Id == authorId && !a.IsArchived)
                ?? throw new InvalidOperationException("Author not found.");

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
            var book = Store.Books.FirstOrDefault(b => b.Id == dto.Id);
            if (book == null) return false;

            if (dto.Title != null) book.Title = dto.Title;
            if (dto.PublishDate != null) book.PublishDate = dto.PublishDate.Value;
            if (dto.Version != null) book.Version = dto.Version;
            if (dto.Publisher != null) book.Publisher = dto.Publisher;
            if (dto.AuthorId != null) book.AuthorId = dto.AuthorId.Value;
            if (dto.CategoryId != null) book.CategoryId = dto.CategoryId.Value;

            book.LastModifiedByUserId = currentUserId;
            book.LastModifiedDate = DateTime.Now;

            return true;
        }

        public bool ArchiveBook(int bookId, int performedByUserId)
        {
            var book = Store.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return false;

            book.IsArchived = true;
            book.ArchivedByUserId = performedByUserId;
            book.ArchivedDate = DateTime.Now;
            return true;
        }

        //Search method (filter, sort, pagination)
        public List<BookListDto> SearchBooks(SearchBookParamsDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var skip = (Math.Max(1, dto.Page) - 1) * Math.Max(1, dto.PageSize);

            var results = Store.Books

                //Search
                .WhereIf(!string.IsNullOrWhiteSpace(dto.SearchParam), b =>
                    (b.Title ?? "").IndexOf(dto.SearchParam!, StringComparison.OrdinalIgnoreCase) >= 0
                    || (Store.Authors.FirstOrDefault(a => a.Id == b.AuthorId)?.Name ?? "")
                        .IndexOf(dto.SearchParam!, StringComparison.OrdinalIgnoreCase) >= 0
                    || (Store.Categories.FirstOrDefault(c => c.Id == b.CategoryId)?.Name ?? "")
                        .IndexOf(dto.SearchParam!, StringComparison.OrdinalIgnoreCase) >= 0
                    || (b.Publisher ?? "").IndexOf(dto.SearchParam!, StringComparison.OrdinalIgnoreCase) >= 0
                )

                //Filter
                .WhereIf(!string.IsNullOrWhiteSpace(dto.Title), b =>
                    (b.Title ?? "").IndexOf(dto.Title!, StringComparison.OrdinalIgnoreCase) >= 0)
                .WhereIf(dto.AuthorId.HasValue, b => b.AuthorId == dto.AuthorId!.Value)
                .WhereIf(dto.CategoryId.HasValue, b => b.CategoryId == dto.CategoryId!.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(dto.Publisher), b =>
                    (b.Publisher ?? "").IndexOf(dto.Publisher!, StringComparison.OrdinalIgnoreCase) >= 0)
                .WhereIf(dto.IsAvailable.HasValue, b =>
                    Store.InventoryRecords.Any(r => r.BookId == b.Id && r.IsAvailable) == dto.IsAvailable!.Value)

                .WhereIf(dto.PublishDate.HasValue, b => b.PublishDate.Date == dto.PublishDate!.Value.Date)
                .ToList();

            //Sorting
            var sortKey = (dto.SortBy ?? "title").Trim().ToLower();
            var desc = dto.SortDir?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

            var keySelector = new Dictionary<string, Func<Book, object>>(StringComparer.OrdinalIgnoreCase)
            {
                ["title"] = b => b.Title ?? "",
                ["publishdate"] = b => b.PublishDate,
                ["author"] = b => Store.Authors.FirstOrDefault(a => a.Id == b.AuthorId)?.Name ?? "",
                ["category"] = b => Store.Categories.FirstOrDefault(c => c.Id == b.CategoryId)?.Name ?? "",
                ["publisher"] = b => b.Publisher ?? "",
                ["isavailable"] = b => Store.InventoryRecords.Any(r => r.BookId == b.Id && r.IsAvailable)
            };

            var selector = keySelector.TryGetValue(sortKey, out var sel) ? sel : keySelector["title"];
            var ordered = desc ? results.OrderByDescending(selector) : results.OrderBy(selector);


            //Pagination + mapping
            return ordered
                .Skip(skip)
                .Take(Math.Max(1, dto.PageSize))
                .Select(b =>
                {
                    var outDto = b.Adapt<BookListDto>();
                    outDto.AuthorName = Store.Authors.FirstOrDefault(a => a.Id == b.AuthorId)?.Name ?? "Unknown";
                    outDto.CategoryName = Store.Categories.FirstOrDefault(c => c.Id == b.CategoryId)?.Name ?? "Unknown";
                    outDto.Publisher = b.Publisher ?? string.Empty;
                    outDto.IsAvailable = Store.InventoryRecords.Any(r => r.BookId == b.Id && r.IsAvailable);
                    return outDto;
                })
                .ToList();
        }
    }
}
