using Contracts.Models;

namespace WebApp.Services.Interfaces
{
    public interface IMyPetsService
    {
        Task<ApiResponse<List<PetDto>>?> GetMyPetsAsync();
        Task<ApiResponse<List<SightingDto>>?> GetMySightingsAsync();
    }
}