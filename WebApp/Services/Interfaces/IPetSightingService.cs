using Contracts.Models;

namespace WebApp.Services.Interfaces
{
    public interface IPetSightingService
    {
        Task<ResponseDto?> CreatePetSightingAsync(SightingDto petDto);
    }
}