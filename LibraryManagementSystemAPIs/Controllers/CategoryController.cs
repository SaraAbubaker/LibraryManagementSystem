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
            var result = Service.GetAllCategories();
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create(CreateCategoryDto dto)
        {
            var created = Service.CreateCategory(dto);
            return Ok(created);
        }

        [HttpPut]
        public IActionResult Update(UpdateCategoryDto dto)
        {
            var currentUserId = 1; //temp until authentication

            var updated = Service.UpdateCategory(dto, currentUserId);

            if (updated == null)
                return NotFound("Category not found.");

            return Ok(updated);
        }

        [HttpPut("delete {id}")]
        public IActionResult Archive(int id)
        {
            var currentUserId = 1; //temp

            var success = Service.ArchiveCategory(id, currentUserId);

            if (!success)
                return NotFound("Category not found.");

            return Ok("Category archived successfully.");
        }
    }
}
