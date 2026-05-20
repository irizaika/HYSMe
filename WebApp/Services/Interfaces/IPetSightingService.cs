using Contracts.Models;

namespace WebApp.Services.Interfaces
{
    public interface IPetSightingService
    {
        Task<ApiResponse<SightingDto>?> CreatePetSightingAsync(SightingDto petDto);
    }
}