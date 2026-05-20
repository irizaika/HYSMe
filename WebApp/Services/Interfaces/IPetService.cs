using Contracts.Models;

namespace WebApp.Services.Interfaces
{
    public interface IPetService
    {
        Task<ApiResponse<List<PetDto>>?> GetAllPetsAsync();
        Task<ApiResponse<PetDto>?> GetPetByIdAsync(int id);
        Task<ApiResponse<PetDto>?> CreatePetAsync(PetDto petDto);
        Task<ApiResponse<PetDto>?> UpdatePetAsync(int id, PetDto petDto);
        Task<ApiResponse<List<PetDto>>?> GetPetsInArea(AreaDto area);
        Task<ApiResponse<List<PetDto>>?> QueryPetsAsync(PetQueryDto filter);


    }
}