using Library.Services.Interfaces;
using Library.Shared.DTOs.Category;
using Library.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet("query")]
        public IActionResult GetAllCategoriesQuery()
        {
            try
            {
                var query = _service.GetAllCategoriesQuery();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory( CreateCategoryDto dto, [FromQuery] int userId)
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
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto dto, [FromQuery] int userId)
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
        public async Task<IActionResult> ArchiveCategory(int id, [FromQuery] int userId)
        {
            try
            {
                var success = await _service.ArchiveCategoryAsync(id, userId);

                if (!success)
                    return NotFound("Category not found.");

                return Ok("Category archived successfully.");
            }
            catch (ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
