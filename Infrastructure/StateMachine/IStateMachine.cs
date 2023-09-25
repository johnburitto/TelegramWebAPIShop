using Infrastructure.Services.Interfaces;

namespace Infrastructure.StateMachine
{
    public interface IStateMachine : ICacheService 
    {
        Task AddStateAsync(string key, State state);
        Task RemoveStateAsync(string key, long userTelegramId);
        Task ChangeStateAsync(string key, State state);
        Task<bool> IsUserHasStateAsync(string key, long userTelegramId);
        Task<State?> GetUserStateAsync(string key, long userTelegramId);
    }
}
