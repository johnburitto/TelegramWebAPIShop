using Infrastructure.StateMachine;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateMachineController : ControllerBase
    {
        private readonly IStateMachine _stateMachine;

        public StateMachineController(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        }


        [HttpGet("{key}")]
        [ProducesResponseType(typeof(List<State>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<State>?>> GetDataAsync(string key)
        {
            var states = await _stateMachine.GetDataAsync<List<State>>(key);

            return Ok(states);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task SetDataAsync()
        {
            await _stateMachine.SetDataAsync("state_machine", new List<State>(), 720);
        }

        [HttpDelete("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task RemoveDataAsync(string key)
        {
            await _stateMachine.RemoveDataAsync(key);
        }

        [HttpPost("add/{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task AddStateAsync(string key, [FromBody] State state)
        {
            await _stateMachine.AddStateAsync(key, state);
        }

        [HttpPost("change/{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task ChangeStateAsync(string key, [FromBody] State state)
        {
            await _stateMachine.ChangeStateAsync(key, state);
        }

        [HttpDelete("{key}/{userTelegramId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task RemoveStateAsync(string key, long userTelegramId)
        {
            await _stateMachine.RemoveStateAsync(key, userTelegramId);
        }

        [HttpGet("{key}/{userTelegramId}/is-has-state")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> IsUserHasStateAsync(string key, long userTelegramId)
        {
            return Ok(await _stateMachine.IsUserHasStateAsync(key, userTelegramId));
        }

        [HttpGet("{key}/{userTelegramId}/state")]
        [ProducesResponseType(typeof(State), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<State>> GetUserStateAsync(string key, long userTelegramId)
        {
            return Ok(await _stateMachine.GetUserStateAsync(key, userTelegramId));
        }
    }
}
