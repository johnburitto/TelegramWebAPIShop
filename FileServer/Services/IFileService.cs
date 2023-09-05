namespace FileServer.Services
{
    public interface IFileService
    {
        Task<bool> SaveAsync(IFormFile file);
        Task<bool> SaveRangeAsync(List<IFormFile> files);
    }
}
