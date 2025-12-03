using LibraryManagementSystem.DTOs.UserType;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetAll()
        {
            try
            {
                var userTypes = _service.GetAllUserTypes();
                return Ok(userTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var userType = _service.GetUserTypeById(id);
                return Ok(userType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateUserTypeDto dto, [FromQuery] int createdByUserId)
        {
            try
            {
                var created = _service.CreateUserType(dto, createdByUserId);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromBody] UpdateUserTypeDto dto, int id, [FromQuery] int userId)
        {
            try
            {
                var updated = _service.UpdateUserType(dto, userId, id);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("archive/{id}")]
        public IActionResult Archive(int id, [FromQuery] int? userId = null)
        {
            try
            {
                var success = _service.ArchiveUserType(id, userId);
                return Ok("User type archived successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
