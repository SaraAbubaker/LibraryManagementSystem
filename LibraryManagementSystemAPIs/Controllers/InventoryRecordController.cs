using LibraryManagementSystem.Entities;
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
            var result = Service.ListCopiesForBook(bookId);
            return Ok(result);
        }

        [HttpGet("available/{bookId}")]
        public IActionResult ListAvailableCopies(int bookId)
        {
            var result = Service.GetAvailableCopies(bookId);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult CreateCopy(int bookId, string copyCode)
        {
            var currentUserId = 1; //temp

            var record = Service.CreateCopy(bookId, copyCode, currentUserId);
            return Ok(record);
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveCopy(int id)
        {
            var currentUserId = 1; //temp

            var success = Service.RemoveCopy(id, currentUserId);

            if (!success)
                return BadRequest("Copy cannot be removed (may be borrowed or not found).");

            return Ok("Copy removed successfully.");
        }

        [HttpPost("return/{inventoryRecordId}")]
        public IActionResult ReturnCopy(int inventoryRecordId)
        {
            var currentUserId = 1; //temp

            var success = Service.ReturnCopy(inventoryRecordId, currentUserId);

            if (!success)
                return NotFound("Inventory record not found.");

            return Ok("Copy returned successfully.");
        }
    }
}
