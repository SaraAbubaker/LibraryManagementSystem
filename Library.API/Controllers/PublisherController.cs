using LibraryManagementSystem.DTOs.Publisher;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublisherController : ControllerBase
    {
        private readonly PublisherService _service;

        public PublisherController(PublisherService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var publishers = await _service.GetAllPublishersAsync();
                return Ok(publishers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var publisher = await _service.GetPublisherByIdAsync(id);
                return Ok(publisher);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePublisherDto dto, [FromQuery] int createdByUserId)
        {
            try
            {
                var created = await _service.CreatePublisherAsync(dto, createdByUserId);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdatePublisherDto dto, int id, [FromQuery] int userId)
        {
            try
            {
                var updated = await _service.UpdatePublisherAsync(dto, userId, id);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("archive/{id}")]
        public async Task<IActionResult> Archive(int id, [FromQuery] int? userId = null)
        {
            try
            {
                if (!userId.HasValue)
                    return BadRequest("UserId is required to archive a publisher.");

                var success = await _service.ArchivePublisherAsync(id, userId.Value);
                return Ok("Publisher archived successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
