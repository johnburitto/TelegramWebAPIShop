namespace FileServer.Services
{
    public class FileService : IFileService
    {
        public async Task<bool> SaveAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Thumbnails", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return true;
            }

            return false;
        }

        public async Task<bool> SaveRangeAsync(List<IFormFile> files)
        {
            foreach (var file in files)
            {
                var result = await SaveAsync(file);

                if (result == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
