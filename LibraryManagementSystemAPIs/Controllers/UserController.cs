using Library.Shared.DTOs.User;
using Library.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            try
            {
                var result = await _service.RegisterUserAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _service.LoginUserAsync(dto);
                return Ok(result);
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
                var query = _service.GetAllUsersQuery();
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
                var query = _service.GetUserByIdQuery(id);
                return Ok(query);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Archive(int id, [FromQuery] int performedByUserId)
        {
            try
            {
                var result = await _service.ArchiveUserAsync(id, performedByUserId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found.");
            }
            catch (InvalidOperationException ex)
            {
                //already archived or has active borrowed books
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
