using Library.Shared.DTOs.UserType;
using Library.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserTypeController : ControllerBase
    {
        private readonly IUserTypeService _service;

        public UserTypeController(IUserTypeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserTypeDto dto, [FromQuery] int createdByUserId)
        {
            try
            {
                var created = await _service.CreateUserTypeAsync(dto, createdByUserId);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateUserTypeDto dto, int id, [FromQuery] int userId)
        {
            try
            {
                var updated = await _service.UpdateUserTypeAsync(dto, userId, id);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("query")]
        public IActionResult GetAllQuery()
        {
            try
            {
                var query = _service.GetAllUserTypesQuery();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("query/{id}")]
        public IActionResult GetByIdQuery(int id)
        {
            try
            {
                var query = _service.GetUserTypeByIdQuery(id);
                return Ok(query);
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
                await _service.ArchiveUserTypeAsync(id, userId);
                return Ok("User type archived successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
