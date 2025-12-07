using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagementSystemAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _service;

        public BooksController(BookService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            try
            {
                var book = await _service.GetBookDetailsAsync(id);
                if (book == null) return NotFound();
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetByAuthor(int authorId)
        {
            try
            {
                var books = await _service.GetBooksByAuthorAsync(authorId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            try
            {
                var books = await _service.GetBooksByCategoryAsync(categoryId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookDto dto, [FromQuery] int userId)
        {
            try
            {
                var book = await _service.CreateBookAsync(dto, userId);
                return CreatedAtAction(nameof(GetDetails), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateBookDto dto, [FromQuery] int userId)
        {
            try
            {
                if (id != dto.Id) return BadRequest("ID mismatch.");

                var success = await _service.UpdateBookAsync(dto, userId);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("archive/{id}")]
        public async Task<IActionResult> Archive(int id, [FromQuery] int userId)
        {
            try
            {
                var success = await _service.ArchiveBookAsync(id, userId);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchBookParamsDto dto)
        {
            try
            {
                var books = await _service.SearchBooksAsync(dto);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
