using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.Services.Impls
{
    public class CartService : ICartService
    {
        private readonly IDistributedCache _redis;

        public CartService(IDistributedCache redis)
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

        public async Task<List<int>?> AddProductToCartAsync(string key, int id)
        {
            var cart = await GetDataAsync<List<int>>(key);

            cart?.Add(id);

            await SetDataAsync(key, cart, 3);

            return await GetDataAsync<List<int>>(key);
        }
    }
}
