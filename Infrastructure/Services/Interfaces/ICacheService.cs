namespace Infrastructure.Services.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetDataAsync<T>(string key);
        Task SetDataAsync<T>(string key, T value, int expirationTimeInHours);
        Task RemoveDataAsync(string key);
    }
}
