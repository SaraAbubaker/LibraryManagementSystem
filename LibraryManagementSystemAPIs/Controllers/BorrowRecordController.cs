using LibraryManagementSystem.DTOs.BorrowRecord;
using LibraryManagementSystem.Entities;
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
            try
            {
                var result = Service.GetBorrowDetails();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("borrow")]
        public IActionResult BorrowBook(RequestBorrowDto dto, [FromQuery] int userId)
        {
            try
            {
                var borrow = Service.BorrowBook(dto, userId);
                return Ok(borrow);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("return/{id}")]
        public IActionResult ReturnBook(int id, [FromQuery] int userId)
        {
            try
            {
                var result = Service.GetBorrowDetails();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("overdue")]
        public IActionResult GetOverdue()
        {
            try
            {
                var result = Service.GetOverdueRecords();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
