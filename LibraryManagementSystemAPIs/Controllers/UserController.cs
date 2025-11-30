using LibraryManagementSystem.DTOs.User;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService Service;

    public UserController(UserService service)
        {
            Service = service;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterUserDto dto)
        {
            var currentUserId = 1; //temp
            var result = Service.RegisterUser(dto, currentUserId);
            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            try
            {
                var result = Service.LoginUser(dto);
                return Ok(result);
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
                var result = Service.GetUserById(id);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found.");
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = Service.GetAllUsers();
            return Ok(result);
        }
    }
}
