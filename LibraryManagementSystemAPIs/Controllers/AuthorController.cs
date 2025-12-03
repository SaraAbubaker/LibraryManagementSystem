using LibraryManagementSystem.DTOs.Author;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Mapster;

namespace LibraryManagementSystemAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly AuthorService _service;

        public AuthorsController(AuthorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListAuthors()
        {
            try
            {
                var result = await _service.ListAuthorsAsync();
                return Ok(result);
            } 
            catch (Exception ex) 
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            try
            {
                var author = await _service.GetAuthorByIdAsync(id);
                if (author == null) return NotFound();

                var dto = author.Adapt<AuthorListDto>();
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuthorListDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAuthor(CreateAuthorDto dto)
        {
            try
            {
                var author = await _service.CreateAuthorAsync(dto);
                return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, author);
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message); 
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditAuthor(int id, UpdateAuthorDto dto)
        {
            try
            {
                if (id != dto.Id) return BadRequest("ID mismatch.");

                var success = await _service.EditAuthorAsync(dto);
                if (!success) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("archive {id}")]
        public async Task<IActionResult> ArchiveAuthor(int id, [FromQuery] int userId)
        {
            try
            {
                var success = await _service.ArchiveAuthorAsync(id, userId);
                if (!success) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
