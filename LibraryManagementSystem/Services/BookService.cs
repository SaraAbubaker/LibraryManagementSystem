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
        //navigation to the lists
        private readonly LibraryDataStore Store;
        public BookService(LibraryDataStore store)
        {
            Store = store;
        }

        public Book CreateBook(CreateBookDto dto, int currentUserId)
        {
            var book = dto.Adapt<Book>();  //Mapping using Mapster

            book.Id = (Store.Books.Select(b => b.Id).DefaultIfEmpty(0).Max()) + 1;

            book.CreatedByUserId = currentUserId;
            book.CreatedDate = DateTime.Now;

            Store.Books.Add(book);
            return book;
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

        public bool DeleteBook(int id)
        {
            var book = Store.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return false;

            //To-do: Remove copies in inventory & clear any inventory record
            Store.Books.Remove(book);
            return true;
        }

        public BookListDto? GetBookDetails(int bookId)
        {
            var book = Store.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return null;

            var dto = book.Adapt<BookListDto>(); //Mapping using Mapster

            dto.AuthorName = Store.Authors
                .FirstOrDefault(a => a.Id == book.AuthorId)?.Name ?? "Unknown";

            dto.CategoryName = Store.Categories
                .FirstOrDefault(c => c.Id == book.CategoryId)?.Name ?? "Unknown";

            dto.IsAvailable = Store.InventoryRecords
                .Any(r => r.BookId == book.Id && r.IsAvailable);

            return dto;
        }

        //To-Do: Search method
    }
}
