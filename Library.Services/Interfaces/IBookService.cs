using Library.Entities.Models;
using Library.Shared.DTOs.Book;

namespace Library.Services.Interfaces
{
    public interface IBookService
    {
        Task<Book> CreateBookAsync(CreateBookDto dto, int currentUserId);
        Task<BookListDto?> GetBookDetailsAsync(int bookId);
        Task<List<BookListDto>> GetBooksByAuthorAsync(int authorId);
        Task<List<BookListDto>> GetBooksByCategoryAsync(int categoryId);
        Task<bool> UpdateBookAsync(UpdateBookDto dto, int currentUserId);
        Task<bool> ArchiveBookAsync(int bookId, int performedByUserId);
        Task<List<BookListDto>> SearchBooksAsync(SearchBookParamsDto dto);
    }
}
