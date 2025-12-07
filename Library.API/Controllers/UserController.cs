using LibraryManagementSystem.DTOs.User;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            try
            {
                var result = await Service.RegisterUserAsync(dto);
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
                var result = await Service.LoginUserAsync(dto);
                return Ok(result);
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
                var result = await Service.GetUserByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await Service.GetAllUsersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Archive(int id, [FromQuery] int performedByUserId)
        {
            try
            {
                var result = await Service.ArchiveUserAsync(id, performedByUserId);
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
