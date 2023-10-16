using Infrastructure.StateMachine;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace TelegramWebAPIShopTest.Tests
{
    public class StateMachineTest
    {
        private readonly IDistributedCache _redis;
        private readonly StateMachine _underTest;

        public StateMachineTest()
        {
            var options = Options.Create(new MemoryDistributedCacheOptions());

            _redis = new MemoryDistributedCache(options);
            _underTest = new StateMachine(_redis);
        }

        [Fact]
        public async void GetDataAsyncTest_NormalFlow()
        {
            // Given
            var key = "1";
            var value = new List<State>()
            {
                new State
                {
                    UserTelegramId = 360604916,
                    CurrentState = "get_phone",
                    StateObject = new()
                }
            };

            await _underTest.SetDataAsync(key, value, 4);

            // When
            var result = await _underTest.GetDataAsync<List<State>>(key);

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<State>>(result);
            Assert.Equal(value.Count, result.Count);
        }

        [Fact]
        public async void GetDataAsyncTest_BadKey()
        {
            // Given
            var key = "1";
            var badKey = "2";
            var value = new List<State>()
            {
                new State
                {
                    UserTelegramId = 360604916,
                    CurrentState = "get_phone",
                    StateObject = new
                    {
                        Phone = ""
                    }
                }
            };

            await _underTest.SetDataAsync(key, value, 4);

            // When
            var result = await _underTest.GetDataAsync<List<State>>(badKey);

            // Then
            Assert.Null(result);
        }

        [Fact]
        public async void SetDataAsyncTest_NormalFlow()
        {
            // Given
            var key = "1";
            var value = new List<State>()
            {
                new State
                {
                    UserTelegramId = 360604916,
                    CurrentState = "get_phone",
                    StateObject = new()
                }
            };

            // When
            await _underTest.SetDataAsync(key, value, 4);

            var result = await _underTest.GetDataAsync<List<State>>(key);

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<State>>(result);
            Assert.Equal(value.Count, result.Count);
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
        public async void AddStateAsyncTest_NormalFlow()
        {
            // Given
            var key = "1";
            var value = new List<State>()
            {
                
            };
            var state = new State
            {
                UserTelegramId = 360604916,
                CurrentState = "get_phone",
                StateObject = new()
            };

            await _underTest.SetDataAsync(key, value, 4);

            // When
            await _underTest.AddStateAsync(key, state);

            var result = await _underTest.GetDataAsync<List<State>>(key);

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<State>>(result);
            Assert.NotEmpty(result);
            Assert.Single(result);
            Assert.Equal(state.UserTelegramId, result[0].UserTelegramId);
            Assert.Equal(state.CurrentState, result[0].CurrentState);
        }

        [Fact]
        public async void AddStateAsyncTest_StateAlreadyExist()
        {
            // Given
            var key = "1";
            var state = new State
            {
                UserTelegramId = 360604916,
                CurrentState = "get_phone",
                StateObject = new()
            };
            var value = new List<State>()
            {
                state
            };

            await _underTest.SetDataAsync(key, value, 4);

            // When
            await _underTest.AddStateAsync(key, state);

            var result = await _underTest.GetDataAsync<List<State>>(key);

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<State>>(result);
            Assert.NotEmpty(result);
            Assert.Single(result);
            Assert.Equal(state.UserTelegramId, result[0].UserTelegramId);
            Assert.Equal(state.CurrentState, result[0].CurrentState);
        }

        [Fact]
        public async void ChangeStateAsyncTest_NormalFlow()
        {
            // Given
            var key = "1";
            var value = new List<State>()
            {
                new State
                {
                    UserTelegramId = 360604916,
                    CurrentState = "get_phone",
                    StateObject = new()
                }
            };
            var state = new State
            {
                UserTelegramId = 360604916,
                CurrentState = "get_address",
                StateObject = new()
            };

            await _underTest.SetDataAsync(key, value, 4);

            // When
            await _underTest.ChangeStateAsync(key, state);

            var result = await _underTest.GetDataAsync<List<State>>(key);

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<State>>(result);
            Assert.NotEmpty(result);
            Assert.Single(result);
            Assert.Equal(state.UserTelegramId, result[0].UserTelegramId);
            Assert.Equal(state.CurrentState, result[0].CurrentState);
        }

        [Fact]
        public async void ChangeStateAsyncTest_UserTelegamIsNotMatch()
        {
            // Given
            var key = "1";
            var value = new List<State>()
            {
                new State
                {
                    UserTelegramId = 360604916,
                    CurrentState = "get_phone",
                    StateObject = new()
                }
            };
            var state = new State
            {
                UserTelegramId = 605990872,
                CurrentState = "get_address",
                StateObject = new()
            };

            await _underTest.SetDataAsync(key, value, 4);

            // When
            var result = await Assert.ThrowsAsync<Exception>(async () => await _underTest.ChangeStateAsync(key, state));

            // Then
            Assert.Equal($"State with user telegram id {state.UserTelegramId} doesn't exist.", result.Message);
        }

        [Fact]
        public async void RemoveStateAsyncTest_NormalFlow()
        {
            // Given
            var key = "1";
            var value = new List<State>()
            {
                new State
                {
                    UserTelegramId = 360604916,
                    CurrentState = "get_phone",
                    StateObject = new()
                }
            };
            var userTelegramId = 360604916;

            await _underTest.SetDataAsync(key, value, 4);

            // When
            await _underTest.RemoveStateAsync(key, userTelegramId);

            var result = await _underTest.GetDataAsync<List<State>>(key);

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<State>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async void RemoveStateAsyncTest_UserTelegamIsNotMatch()
        {
            // Given
            var key = "1";
            var state = new State
            {
                UserTelegramId = 360604916,
                CurrentState = "get_phone",
                StateObject = new()
            };
            var value = new List<State>()
            {
                state
            };
            var userTelegramId = 605990872;

            await _underTest.SetDataAsync(key, value, 4);

            // When
            await _underTest.RemoveStateAsync(key, userTelegramId);

            var result = await _underTest.GetDataAsync<List<State>>(key);

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<State>>(result);
            Assert.NotEmpty(result);
            Assert.Single(result);
            Assert.Equal(state.UserTelegramId, result[0].UserTelegramId);
            Assert.Equal(state.CurrentState, result[0].CurrentState);
        }

        [Fact]
        public async void IsUserHasStateAsyncTest_UserHasState()
        {
            // Given
            var key = "1";
            var state = new State
            {
                UserTelegramId = 360604916,
                CurrentState = "get_phone",
                StateObject = new()
            };
            var value = new List<State>()
            {
                state
            };
            var userTelegramId = 360604916;

            await _underTest.SetDataAsync(key, value, 4);

            // When
            var result = await _underTest.IsUserHasStateAsync(key, userTelegramId);

            // Then
            Assert.True(result);
        }

        [Fact]
        public async void IsUserHasStateAsyncTest_UserHasNoState()
        {
            // Given
            var key = "1";
            var value = new List<State>()
            {
                
            };
            var userTelegramId = 360604916;

            await _underTest.SetDataAsync(key, value, 4);

            // When
            var result = await _underTest.IsUserHasStateAsync(key, userTelegramId);

            // Then
            Assert.False(result);
        }

        [Fact]
        public async void GetUserStateAsyncTest_UserHasState()
        {
            // Given
            var key = "1";
            var state = new State
            {
                UserTelegramId = 360604916,
                CurrentState = "get_phone",
                StateObject = new()
            };
            var value = new List<State>()
            {
                state
            };
            var userTelegramId = 360604916;

            await _underTest.SetDataAsync(key, value, 4);

            // When
            var result = await _underTest.GetUserStateAsync(key, userTelegramId);

            // Then
            Assert.NotNull(result);
            Assert.IsType<State>(result);
            Assert.Equal(userTelegramId, result.UserTelegramId);
            Assert.Equal("get_phone", result.CurrentState);
        }

        [Fact]
        public async void GetUserStateAsyncTest_UserHasNoState()
        {
            // Given
            var key = "1";
            var value = new List<State>()
            {

            };
            var userTelegramId = 360604916;

            await _underTest.SetDataAsync(key, value, 4);

            // When
            var result = await _underTest.GetUserStateAsync(key, userTelegramId);

            // Then
            Assert.Null(result);
        }
    }
}