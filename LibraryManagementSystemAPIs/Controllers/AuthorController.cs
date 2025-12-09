using Library.Services.Interfaces;
using Library.Shared.DTOs.Author;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _service;

        public AuthorsController(IAuthorService service)
        {
            _service = service;
        }

        [HttpGet("query")]
        public IActionResult ListAuthorsQuery()
        {
            try
            {
                var result = _service.ListAuthorsQuery();
                return Ok(result);
            } 
            catch (Exception ex) 
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("query/{id}")]
        public async Task<IActionResult> GetAuthorByIdQuery(int id)
        {
            try
            {
                var query = _service.GetAuthorByIdQuery(id);
                var author = await query.FirstOrDefaultAsync();
                if (author == null) return NotFound();

                return Ok(author);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuthorListDto), 201)]
        [ProducesResponseType(400)]
        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorDto dto, [FromQuery] int userId)
        {
            try
            {
                if (dto == null) return BadRequest("Author data is required.");

                var author = await _service.CreateAuthorAsync(dto, userId);
                return CreatedAtAction(nameof(GetAuthorByIdQuery), new { id = author.Id }, author);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditAuthor(int id, [FromBody] UpdateAuthorDto dto, [FromQuery] int userId)
        {
            try
            {
                if (id != dto.Id) return BadRequest("ID mismatch.");

                var success = await _service.EditAuthorAsync(dto, userId);
                if (!success) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("archive/{id}")]
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
