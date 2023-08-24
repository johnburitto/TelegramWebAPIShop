namespace Infrastructure.Services.Interfaces
{
    public interface ICartService : ICacheService
    {
        Task<List<int>?> AddProductToCartAsync(string key, int id);
    }
}
