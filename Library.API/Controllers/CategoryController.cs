using LibraryManagementSystem.DTOs.Category;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoryController(CategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllCategoriesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create( CreateCategoryDto dto, [FromQuery] int userId)
        {
            try
            {
                var created = await _service.CreateCategoryAsync(dto, userId);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateCategoryDto dto, [FromQuery] int userId)
        {
            try
            {
                var updated = await _service.UpdateCategoryAsync(dto, userId);

                if (updated == null)
                    return NotFound("Category not found.");

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
                var success = await _service.ArchiveCategoryAsync(id, userId);

                if (!success)
                    return NotFound("Category not found.");

                return Ok("Category archived successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
