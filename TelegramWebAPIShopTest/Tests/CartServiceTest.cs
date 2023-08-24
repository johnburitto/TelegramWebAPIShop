using Infrastructure.Services.Impls;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace TelegramWebAPIShopTest.Tests
{
    public class CartServiceTest
    {
        private readonly IDistributedCache _redis;
        private readonly CartService _underTest;

        public CartServiceTest()
        {
            var options = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
            
            _redis = new MemoryDistributedCache(options);
            _underTest = new CartService(_redis);
        }

        [Fact]
        public async void GetDataAsyncTest_NormalFlow()
        {
            // Given
            var key = "1";
            var value = new List<int> { 1, 2, 3 };

            await _underTest.SetDataAsync(key, value, 4);

            // When
            var result = await _underTest.GetDataAsync<List<int>>(key);

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<int>>(result);
            Assert.Equal(value.Count, result.Count);
            Assert.Equal(value, result);
        }

        [Fact]
        public async void GetDataAsyncTest_BadKey()
        {
            // Given
            var key = "1";
            var badKey = "2";
            var value = new List<int> { 1, 2, 3 };

            await _underTest.SetDataAsync(key, value, 4);

            // When
            var result = await _underTest.GetDataAsync<List<int>>(badKey);

            // Then
            Assert.Null(result);
        }

        [Fact]
        public async void SetDataAsyncTest_NormalFlow()
        {
            // Given
            var key = "1";
            var value = new List<int> { 1, 2, 3 };

            // When
            await _underTest.SetDataAsync(key, value, 4);

            var result = await _underTest.GetDataAsync<List<int>>(key);

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<int>>(result);
            Assert.Equal(value.Count, result.Count);
            Assert.Equal(value, result);
        }

        [Fact]
        public async void RemoveDataAsyncTest_NormalFlow()
        {
            // Given
            var key = "1";

            // When
            await _underTest.RemoveDataAsync(key);

            // Then
        }

        [Fact]
        public async void AddProductToCartAsyncTest_NormalFlow()
        {
            // Given
            var key = "1";
            var value = new List<int> { 1, 2, 3 };
            var newValue = 4;

            await _underTest.SetDataAsync(key, value, 4);

            // When
            var result = await _underTest.AddProductToCartAsync(key, newValue);

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<int>>(result);
            Assert.Equal(value.Count + 1, result.Count);
            Assert.Contains(newValue, result);
        }

        [Fact]
        public async void AddProductToCartAsyncTest_BadKey()
        {
            // Given
            var key = "1";
            var badKey = "2";
            var value = new List<int> { 1, 2, 3 };
            var newValue = 4;

            await _underTest.SetDataAsync(key, value, 4);

            // When
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _underTest.AddProductToCartAsync(badKey, newValue));

            // Then
            Assert.Equal($"Value cannot be null. (Parameter 'There is no cart with user id {badKey}')", result.Message);
        }
    }
}
