using LibraryManagementSystem.DTOs.Book;
using LibraryManagementSystem.Entities;
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
                var result = Service.GetBookDetails(id);
                if (result == null) return NotFound();
                return Ok(result);
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
                var result = Service.GetBooksByAuthor(authorId);
                return Ok(result);
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
                var result = Service.GetBooksByCategory(categoryId);
                return Ok(result);
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
                var result = Service.CreateBook(dto, userId);
                return CreatedAtAction(nameof(GetDetails), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Update(UpdateBookDto dto, [FromQuery] int userId)
        {
            try
            {
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
                var result = Service.SearchBooks(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
