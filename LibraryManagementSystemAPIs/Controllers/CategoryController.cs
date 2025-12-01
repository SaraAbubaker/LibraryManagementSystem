using LibraryManagementSystem.DTOs.Category;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService Service;

        public CategoryController(CategoryService service)
        {
            Service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var result = Service.GetAllCategories();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create(CreateCategoryDto dto)
        {
            try
            {
                var created = Service.CreateCategory(dto);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Update(UpdateCategoryDto dto, [FromQuery] int userId)
        {
            try
            {
                var updated = Service.UpdateCategory(dto, userId);

                if (updated == null)
                    return NotFound("Category not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("archive {id}")]
        public IActionResult Archive(int id, [FromQuery] int userId)
        {
            try
            {
                var success = Service.ArchiveCategory(id, userId);

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
