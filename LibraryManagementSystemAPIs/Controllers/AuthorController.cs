using LibraryManagementSystem.DTOs.Author;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly AuthorService Service;

        public AuthorsController(AuthorService service)
        {
            Service = service;
        }

        [HttpGet]
        public IActionResult ListAuthors()
        {
            var result = Service.ListAuthors();
            return Ok(result);
        }

        [HttpPost]
        public IActionResult CreateAuthor(CreateAuthorDto dto)
        {
            var result = Service.CreateAuthor(dto);
            return CreatedAtAction(nameof(ListAuthors), result);
        }

        [HttpPut]
        public IActionResult EditAuthor(UpdateAuthorDto dto)
        {
            var success = Service.EditAuthor(dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult ArchiveAuthor(int id)
        {
            int currentUserId = 1;

            var success = Service.ArchiveAuthor(id, currentUserId);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
