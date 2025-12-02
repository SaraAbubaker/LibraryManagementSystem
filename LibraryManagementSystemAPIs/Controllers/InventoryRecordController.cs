using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService Service;

        public InventoryController(InventoryService service)
        {
            Service = service;
        }

        [HttpGet("book/{bookId}")]
        public IActionResult ListCopies(int bookId)
        {
            try
            {
                var result = Service.ListCopiesForBook(bookId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("available/{bookId}")]
        public IActionResult ListAvailableCopies(int bookId)
        {
            try
            {
                var result = Service.GetAvailableCopies(bookId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateCopy(int bookId, string copyCode, [FromQuery] int userId)
        {
            try
            {
                var record = Service.CreateCopy(bookId, copyCode, userId);
                return Ok(record);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("archive {id}")]
        public IActionResult RemoveCopy(int id, [FromQuery] int userId)
        {
            try
            {
                var success = Service.RemoveCopy(id, userId);

                if (!success)
                    return BadRequest("Copy cannot be removed (may be borrowed or not found).");

                return Ok("Copy removed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("return/{inventoryRecordId}")]
        public IActionResult ReturnCopy(int inventoryRecordId, [FromQuery] int userId)
        {
            try
            {
                var success = Service.ReturnCopy(inventoryRecordId, userId);

                if (!success)
                    return NotFound("Inventory record not found.");

                return Ok("Copy returned successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
