using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class FileService : IFileService
    {
        public async Task<string?> SaveImageAsync(IFormFile? file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{folder}");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/{folder}/{fileName}";
        }
    }
}
