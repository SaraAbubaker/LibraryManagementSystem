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
        public BookDto MapToBookDto(BookDto dto)
        {
            return new BookDto
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
                AvailableCopies = dto.AvailableCopies,

                CreatedByUserId = dto.CreatedByUserId,
                CreatedDate = dto.CreatedDate,
                LastModifiedByUserId = dto.LastModifiedByUserId,
                LastModifiedDate = dto.LastModifiedDate
            };
        }

        public BookListDto MapToBookListDto(BookListDto dto)
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

        public Book MapToCreateBookDto(CreateBookDto dto)
        {
            return new Book
            {
                Title = dto.Title,
                PublishDate = dto.PublishDate,
                Version = dto.Version,
                Publisher = dto.Publisher,

                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId
            };
        }

        public SearchBookParamsDto MapToSearchBookParamsDto(SearchBookParamsDto dto)
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
                Publisher = dto.Publisher,

                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                IsAvailable = dto.IsAvailable
            };
        }

        public UpdateBookDto MapToUpdateBookDto(UpdateBookDto dto)
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
