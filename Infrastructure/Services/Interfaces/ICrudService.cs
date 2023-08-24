namespace Infrastructure.Services.Interfaces
{
    public interface ICrudService<T, TCreate, TUpdate>
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(TCreate dto);
        Task<T> UpdateAsync(TUpdate dto);
        Task DeleteAsync(T entity);
    }
}
