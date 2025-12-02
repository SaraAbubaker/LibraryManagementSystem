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
        private readonly AuthorService Service;

        public AuthorsController(AuthorService service)
        {
            Service = service;
        }

        [HttpGet]
        public IActionResult ListAuthors()
        {
            try 
            { 
                var result = Service.ListAuthors();
                return Ok(result);
            } 
            catch (Exception ex) 
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthorById(int id)
        {
            try
            {
                var author = Service.GetAuthorById(id);
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
        public IActionResult CreateAuthor(CreateAuthorDto dto)
        {
            try 
            {
                var author = Service.CreateAuthor(dto);
                var resultDto = author.Adapt<AuthorListDto>();

                return CreatedAtAction(nameof(GetAuthorById), new { id = resultDto.Id }, resultDto);
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message); 
            }
        }

        [HttpPut]
        public IActionResult EditAuthor(int id, UpdateAuthorDto dto)
        {
            try 
            {
                if (id != dto.Id) return BadRequest("ID mismatch.");

                var success = Service.EditAuthor(dto);
                if (!success) return NotFound();

                return NoContent();
            } 
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("archive {id}")]
        public IActionResult ArchiveAuthor(int id, [FromQuery] int userId)
        {
            try
            {
                var success = Service.ArchiveAuthor(id, userId);
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
