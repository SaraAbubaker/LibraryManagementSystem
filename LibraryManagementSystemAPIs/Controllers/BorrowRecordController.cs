using LibraryManagementSystem.DTOs.BorrowRecord;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowRecordController : ControllerBase
    {
        private readonly BorrowService Service;

        public BorrowRecordController(BorrowService service)
        {
            Service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = Service.GetBorrowDetails();
            return Ok(result);
        }

        [HttpPost("borrow")]
        public IActionResult Borrow(RequestBorrowDto dto)
        {
            // In the future, replace 1 with authenticated user ID
            var borrow = Service.BorrowBook(dto);
            return Ok(borrow);
        }

        [HttpPost("return/{id}")]
        public IActionResult ReturnBook(int id)
        {
            var currentUserId = 1; //temp

            var success = Service.ReturnBook(id, currentUserId);

            if (!success)
                return BadRequest("Borrow record not found or already returned.");

            return Ok("Book returned successfully.");
        }

        [HttpGet("overdue")]
        public IActionResult GetOverdue()
        {
            var result = Service.GetOverdueRecords();
            return Ok(result);
        }
    }
}
