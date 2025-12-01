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
            var result = Service.GetBookDetails(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("author/{authorId}")]
        public IActionResult GetByAuthor(int authorId)
        {
            var result = Service.GetBooksByAuthor(authorId);
            return Ok(result);
        }
        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            var result = Service.GetBooksByCategory(categoryId);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create(CreateBookDto dto)
        {
            int currentUserId = 1; //temp

            var result = Service.CreateBook(dto, currentUserId);

            return CreatedAtAction(nameof(GetDetails), new { id = result.Id }, result);
        }

        [HttpPut]
        public IActionResult Update(UpdateBookDto dto)
        {
            int currentUserId = 1; //temp

            var success = Service.UpdateBook(dto, currentUserId);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPut("delete {id}")]
        public IActionResult Archive(int id)
        {
            int currentUserId = 1; //temp

            var success = Service.ArchiveBook(id, currentUserId);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] SearchBookParamsDto dto)
        {
            var result = Service.SearchBooks(dto);
            return Ok(result);
        }
    }
}
