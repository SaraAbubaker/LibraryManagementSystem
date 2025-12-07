using Library.Shared.DTOs.BorrowRecord;
using Library.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowRecordController : ControllerBase
    {
        private readonly IBorrowService _service;

        public BorrowRecordController(IBorrowService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetBorrowDetailsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook(RequestBorrowDto dto, [FromQuery] int userId)
        {
            try
            {
                var borrow = await _service.BorrowBookAsync(dto, userId);
                return Ok(borrow);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("return/{borrowRecordId}")]
        public async Task<IActionResult> ReturnBook(int borrowRecordId, [FromQuery] int userId)
        {
            try
            {
                var success = await _service.ReturnBookAsync(borrowRecordId, userId);
                if (!success) return NotFound();
                return Ok(new { Message = "Book returned successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdue()
        {
            try
            {
                var result = await _service.GetOverdueRecordsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
