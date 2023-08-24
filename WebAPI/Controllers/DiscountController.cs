using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _service;

        public DiscountController(IDiscountService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Discount>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Discount>>> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}", Name = "GetDiscountByIdAsync")]
        [ProducesResponseType(typeof(List<Discount>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Discount?>> GetByIdAsync(int id)
        {
            var discount = await _service.GetByIdAsync(id);

            return discount != null ? Ok(discount) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<Discount>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Discount>> CreateAsync([FromBody] DiscountCreateDto dto)
        {
            var discount = await _service.CreateAsync(dto);

            return CreatedAtRoute("GetDiscountByIdAsync", new { discount.Id }, discount);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(List<Discount>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Discount>> UpdateAsync(int id, [FromBody] DiscountUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var discount = await _service.UpdateAsync(dto);

            return discount != null ? Ok(discount) : NotFound();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var discount = await _service.GetByIdAsync(id);

            if (discount == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(discount);

            return NoContent();
        }
    }
}
