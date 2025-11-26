using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class BookService
    {
        


        //Mapping
        public BookDetailsDto MapToBookDetailsDto(BookDetailsDto dto)
        {
            return new BookDetailsDto
            {
                Id = dto.Id,
                Title = dto.Title,
                PublishDate = dto.PublishDate,
                Version = dto.Version,
                Publisher = dto.Publisher,
                AuthorId = dto.AuthorId,
                AuthorName = dto.AuthorName,
                CategoryId = dto.CategoryId,
                CategoryName = dto.CategoryName,
                TotalCopies = dto.TotalCopies,
                AvailableCopies = dto.AvailableCopies
            };
        }

        public BookListDto MapToBookList(BookListDto dto)
        {
            return new BookListDto
            {
                Id = dto.Id,
                Title = dto.Title,
                PublishDate = dto.PublishDate,
                Version = dto.Version,
                Publisher = dto.Publisher,
                AuthorName = dto.AuthorName,
                CategoryName = dto.CategoryName,
                IsAvailable = dto.IsAvailable,
                CreatedByUserId = dto.CreatedByUserId,
                CreatedDate = dto.CreatedDate,
                LastModifiedByUserId = dto.LastModifiedByUserId,
                LastModifiedDate = dto.LastModifiedDate
            };
        }

        public Book MapToCreateBook(CreateBookDto dto)
        {
            return new Book
            {
                Title = dto.Title,
                PublishDate = dto.PublishDate,
                Version = dto.Version,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                Publisher = dto.Publisher
            };
        }

        public SearchBookParamsDto MapToSearchBookParams(SearchBookParamsDto dto)
        {
            return new SearchBookParamsDto
            {
                Page = dto.Page,
                PageSize = dto.PageSize,
                SortBy = dto.SortBy,
                SortDir = dto.SortDir,
                Title = dto.Title,
                PublishDate = dto.PublishDate,
                Version = dto.Version,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                Publisher = dto.Publisher,
                IsAvailable = dto.IsAvailable
            };
        }

        public UpdateBookDto MapToUpdateBook(UpdateBookDto dto)
        {
            return new UpdateBookDto 
            { 
                Id = dto.Id,
                Title = dto.Title,
                PublishDate = dto.PublishDate,
                Version = dto.Version,
                Publisher = dto.Publisher,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId
            };
        }
    }
}
