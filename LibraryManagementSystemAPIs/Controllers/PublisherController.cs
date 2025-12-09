using Library.Shared.DTOs.Publisher;
using Library.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublisherController : ControllerBase
    {
        private readonly IPublisherService _service;

        public PublisherController(IPublisherService service)
        {
            _service = service;
        }

        [HttpGet("query")]
        public IActionResult GetAllQuery()
        {
            try
            {
                var query = _service.GetAllPublishersQuery();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("query/{id}")]
        public IActionResult GetByIdQuery(int id)
        {
            try
            {
                var query = _service.GetPublisherByIdQuery(id);
                return Ok(query);
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
