using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Order>>> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}", Name = "GetOrderByIdAsync")]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Order?>> GetByIdAsync(int id)
        {
            var order = await _service.GetByIdAsync(id);

            return order != null ? Ok(order) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Order>> CreateAsync([FromBody] OrderCreateDto dto)
        {
            var order = await _service.CreateAsync(dto);

            return CreatedAtRoute("GetOrderByIdAsync", new { order.Id }, order);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Order>> UpdateAsync(int id, [FromBody] OrderUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var order = await _service.UpdateAsync(dto);

            return order != null ? Ok(order) : NotFound();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var order = await _service.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(order);

            return NoContent();
        }
    }
}
