using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        //Dependancy injection
        private readonly BookService Service;
        //Dependancy constructor
        public BooksController(BookService service)
        {
            Service = service;
        }

        [HttpGet("{id}")]
        public IActionResult GetDetails(int id)
        {
            try
            {
                var book = Service.GetBookDetails(id);
                if (book == null) return NotFound();
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("author/{authorId}")]
        public IActionResult GetByAuthor(int authorId)
        {
            try
            {
                var book = Service.GetBooksByAuthor(authorId);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            try
            {
                var book = Service.GetBooksByCategory(categoryId);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create(CreateBookDto dto, [FromQuery] int userId)
        {
            try
            {
                var book = Service.CreateBook(dto, userId);
                return CreatedAtAction(nameof(GetDetails), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateBookDto dto, [FromQuery] int userId)
        {
            try
            {
                if (id != dto.Id) return BadRequest("ID mismatch.");

                var success = Service.UpdateBook(dto, userId);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("archive {id}")]
        public IActionResult Archive(int id, [FromQuery] int userId)
        {
            try
            {
                var success = Service.ArchiveBook(id, userId);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] SearchBookParamsDto dto)
        {
            try
            {
                var book = Service.SearchBooks(dto);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
