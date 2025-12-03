using LibraryManagementSystem.DTOs.UserType;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserTypeController : ControllerBase
    {
        private readonly UserTypeService _service;

        public UserTypeController(UserTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userTypes = await _service.GetAllUserTypesAsync();
                return Ok(userTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userType = await _service.GetUserTypeByIdAsync(id);
                return Ok(userType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
