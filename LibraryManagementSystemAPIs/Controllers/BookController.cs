using Library.Services.Interfaces;
using Library.Shared.DTOs.Book;
using Library.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _service;

        public BooksController(IBookService service)
        {
            _service = service;
        }

        [HttpGet("query/{id}")]
        public async Task<IActionResult> GetBookDetailsQuery(int id)
        {
            try
            {
                var query = _service.GetBookDetailsQuery(id);
                var book = await query.FirstOrDefaultAsync(); // Execute query
                if (book == null) return NotFound();

                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("query/author/{authorId}")]
        public IActionResult GetBooksByAuthorQuery(int authorId)
        {
            try
            {
                var query = _service.GetBooksByAuthorQuery(authorId);
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("query/category/{categoryId}")]
        public IActionResult GetBooksByCategoryQuery(int categoryId)
        {
            try
            {
                var query = _service.GetBooksByCategoryQuery(categoryId);
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto dto, [FromQuery] int userId)
        {
            try
            {
                var book = await _service.CreateBookAsync(dto, userId);
                return CreatedAtAction(nameof(GetBookDetailsQuery), new { id = book.Id }, book); // Match service method
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
        public async Task<IActionResult> ArchiveBook(int id, [FromQuery] int userId)
        {
            try
            {
                var success = await _service.ArchiveBookAsync(id, userId);
                if (!success) return NotFound();

                var query = _service.GetBookDetailsQuery(id);
                var book = await query.FirstOrDefaultAsync();
                return Ok(book);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("query/search")]
        public IActionResult SearchBooksQuery([FromQuery] SearchBookParamsDto dto)
        {
            try
            {
                var query = _service.SearchBooksQuery(dto);
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
