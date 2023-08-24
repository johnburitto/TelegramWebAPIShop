using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThumbnailController : ControllerBase
    {
        private readonly IThumbnailService _service;

        public ThumbnailController(IThumbnailService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Thumbnail>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Thumbnail>>> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}", Name = "GetThumbnailByIdAsync")]
        [ProducesResponseType(typeof(List<Thumbnail>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Thumbnail?>> GetByIdAsync(int id) 
        {
            var thumbnail = await _service.GetByIdAsync(id);

            return thumbnail != null ? Ok(thumbnail) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<Thumbnail>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Thumbnail>> CreateAsync([FromBody] ThumbnailCreateDto dto)
        {
            var thumbnail = await _service.CreateAsync(dto);

            return CreatedAtRoute("GetThumbnailByIdAsync", new { thumbnail.Id }, thumbnail);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(List<Thumbnail>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Thumbnail>> UpdateAsync(int id, [FromBody] ThumbnailUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var thumbnail = await _service.UpdateAsync(dto);

            return thumbnail != null ? Ok(thumbnail) : NotFound();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var thumbnail = await _service.GetByIdAsync(id);

            if (thumbnail == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(thumbnail);

            return NoContent();
        }
    }
}
