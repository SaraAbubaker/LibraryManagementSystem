using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _service;

        public InventoryController(InventoryService service)
        {
            _service = service;
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> ListCopies(int bookId)
        {
            try
            {
                var result = await _service.ListCopiesForBookAsync(bookId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("available/{bookId}")]
        public async Task<IActionResult> ListAvailableCopies(int bookId)
        {
            try
            {
                var result = await _service.GetAvailableCopiesAsync(bookId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCopy(int bookId, string copyCode, [FromQuery] int userId)
        {
            try
            {
                var record = await _service.CreateCopyAsync(bookId, copyCode, userId);
                return Ok(record);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("archive/{id}")]
        public async Task<IActionResult> RemoveCopy(int id, [FromQuery] int userId)
        {
            try
            {
                var success = await _service.RemoveCopyAsync(id, userId);

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
        public async Task<IActionResult> ReturnCopy(int inventoryRecordId, [FromQuery] int userId)
        {
            try
            {
                var success = await _service.ReturnCopyAsync(inventoryRecordId, userId);

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
