using Core.Dtos.Create;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet("{key}")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<int>?>> GetDataAsync(string key)
        {
            var cart = await _service.GetDataAsync<List<int>>(key);

            return Ok(cart);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task SetDataAsync([FromBody] CartCreateDto dto)
        {
            await _service.SetDataAsync(dto.UserTelegramId.ToString() ?? throw new ArgumentNullException(nameof(dto.UserTelegramId)), 
                dto.Products, dto.ExpirationTimeInHours);
        }

        [HttpPost("add/{key}/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<int>?>> AddProductToCartAsync(string key, int id)
        {
            return Ok(await _service.AddProductToCartAsync(key, id));
        }

        [HttpDelete("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task RemoveDataAsync(string key)
        {
            await _service.RemoveDataAsync(key);
        }

        [HttpDelete("{key}/{id}/{isRemoveAll}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task RemoveProductFromCartAsync(string key, int id, bool isRemoveAll)
        {
            await _service.RemoveProductFromCartAsync(key, id, isRemoveAll);
        }
    }
}
