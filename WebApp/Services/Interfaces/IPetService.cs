using Contracts.Models;

namespace WebApp.Services.Interfaces
{
    public interface IPetService
    {
        Task<ResponseDto?> GetAllPetsAsync();
        Task<ResponseDto?> GetPetByIdAsync(int id);
        Task<ResponseDto?> CreatePetAsync(PetDto petDto);
        Task<ResponseDto?> UpdatePetAsync(int id, PetDto petDto);
        Task<ResponseDto?> GetPetsInArea(AreaDto area);
        Task<ResponseDto?> QueryPetsAsync(PetQueryDto filter);


    }
}