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
        
        [HttpPost]
        [ProducesResponseType(typeof(AuthorListDto), 201)]
        [ProducesResponseType(400)]
        public IActionResult CreateAuthor(CreateAuthorDto dto)
        {
            try 
            { 
                var result = Service.CreateAuthor(dto);
                return CreatedAtAction(nameof(ListAuthors), result);
            } 
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message); 
            }
        }

        [HttpPut]
        public IActionResult EditAuthor(UpdateAuthorDto dto)
        {
            try 
            { 
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
