using LibraryManagementSystem.DTOs.Publisher;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetAll()
        {
            try
            {
                var publishers = _service.GetAllPublishers();
                return Ok(publishers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var publisher = _service.GetPublisherById(id);
                return Ok(publisher);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreatePublisherDto dto, [FromQuery] int createdByUserId)
        {
            try
            {
                var created = _service.CreatePublisher(dto, createdByUserId);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromBody] UpdatePublisherDto dto, int id, [FromQuery] int userId)
        {
            try
            {
                var updated = _service.UpdatePublisher(dto, userId, id);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("archive/{id}")]
        public IActionResult Archive(int id, [FromQuery] int? userId = null)
        {
            try
            {
                var success = _service.ArchivePublisher(id, userId);
                return Ok("Publisher archived successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
