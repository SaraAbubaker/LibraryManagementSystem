using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

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

        //To-Do: Search method (filter, sort, pagination)

        //Searches title
        public List<Book> SearchBooks(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return new List<Book>();

            return Store.Books
                .Where(b => !string.IsNullOrEmpty(b.Title) &&
                            b.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
    }
}
