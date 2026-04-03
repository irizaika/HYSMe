namespace WebApp.Services.Interfaces
{
    public interface IFileService
    {
        Task<string?> SaveImageAsync(IFormFile? file, string folder);
        // todo delete unused picture
    }
}
