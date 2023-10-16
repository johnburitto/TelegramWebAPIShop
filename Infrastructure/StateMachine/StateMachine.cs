using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.StateMachine
{
    public class StateMachine : IStateMachine
    {
        private readonly IDistributedCache _redis;

        public StateMachine(IDistributedCache redis)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {
            var value = await _redis.GetStringAsync(key);

            return !string.IsNullOrEmpty(value) ? JsonConvert.DeserializeObject<T>(value) : default;
        }

        public async Task SetDataAsync<T>(string key, T value, int expirationTimeInHours)
        {
            var valueString = JsonConvert.SerializeObject(value);
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(expirationTimeInHours))
                .SetAbsoluteExpiration(DateTime.Now.AddHours(expirationTimeInHours));

            await _redis.SetStringAsync(key, valueString, options);
        }

        public async Task RemoveDataAsync(string key)
        {
            await _redis.RemoveAsync(key);
        }

        public async Task AddStateAsync(string key, State state)
        {
            var states = await GetDataAsync<List<State>>(key);

            if (states?.Where(s => s.UserTelegramId == state.UserTelegramId).FirstOrDefault() == null)
            {
                states?.Add(state);
            }
            else
            {
                return;
            }

            await SetDataAsync(key, states, 720);
        }

        public async Task ChangeStateAsync(string key, State state)
        {
            var states = await GetDataAsync<List<State>>(key);
            var currentState = states!.Find(s => s.UserTelegramId == state.UserTelegramId) ?? 
                throw new Exception($"State with user telegram id {state.UserTelegramId} doesn't exist.");

            states.Remove(currentState);
            states.Add(state);

            await SetDataAsync(key, states, 720);
        }

        public async Task RemoveStateAsync(string key, long userTelegramId)
        {
            var states = await GetDataAsync<List<State>>(key);
            var state = states?.Where(s => s.UserTelegramId == userTelegramId).FirstOrDefault();

            if (state != null)
            {
                states?.Remove(state);
            }

            await SetDataAsync(key, states, 720);
        }

        public async Task<bool> IsUserHasStateAsync(string key, long userTelegramId)
        {
            var states = await GetDataAsync<List<State>>(key);
            var state = states?.Where(s => s.UserTelegramId == userTelegramId).FirstOrDefault();

            return state != null;
        }

        public async Task<State?> GetUserStateAsync(string key, long userTelegramId)
        {
            var states = await GetDataAsync<List<State>>(key);
            var state = states?.Where(s => s.UserTelegramId == userTelegramId).FirstOrDefault();

            return state;
        }
    }
}
