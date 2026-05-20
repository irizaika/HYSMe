using Contracts.Enums;
using Contracts.Models;
using WebApp.Services.Interfaces;
using WebApp.Utility;

namespace WebApp.Services
{
    public class PetService : IPetService
    {
        private readonly IBaseService _baseService;
        public PetService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ApiResponse<List<PetDto>>?> GetAllPetsAsync()
        {
            return await _baseService.SendAsync<List<PetDto>>(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = SD.APIBase + "/api/pets"
            });
        }

        public async Task<ApiResponse<PetDto>?> GetPetByIdAsync(int id)
        {
            return await _baseService.SendAsync<PetDto>(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = SD.APIBase + $"/api/pets/{id}"
            });
        }

        public async Task<ApiResponse<PetDto>?> CreatePetAsync(PetDto petDto)
        {
            ApiResponse<PetDto>? responce;
            try
            {
                responce = await _baseService.SendAsync<PetDto>(new RequestDto
                {
                    ApiType = ApiType.POST,
                    Data = petDto,
                    Url = SD.APIBase + "/api/pets"
                });

                return responce;
            }
            catch
            {
                return new ApiResponse<PetDto>()
                {
                    IsSuccess = false,
                    Message = "Something went wrong while creating the pet. Please try again later.",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<PetDto>?> UpdatePetAsync(int id, PetDto petDto)
        {
            return await _baseService.SendAsync<PetDto>(new RequestDto
            {
                ApiType = ApiType.PUT,
                Data = petDto,
                Url = SD.APIBase + $"/api/pets/{id}"
            });
        }

        public async Task<ApiResponse<List<PetDto>>?> GetPetsInArea(AreaDto area)
        {
            return await _baseService.SendAsync<List<PetDto>>(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{SD.APIBase}/api/pets/area?north={area.North}&south={area.South}&east={area.East}&west={area.West}"
            });
        }

        public async Task<ApiResponse<List<PetDto>>?> QueryPetsAsync(PetQueryDto filter)
        {
            string statusQuery = "";
            if (filter.Statuses != null && filter.Statuses.Count > 0)
            {
                statusQuery = string.Join("&", filter.Statuses.Select(s => $"statuses={s}"));
            }

            return await _baseService.SendAsync<List<PetDto>>(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{SD.APIBase}/api/pets/query?" +
                      $"itemPerPage={filter.ItemPerPage}&pageNumber={filter.PageNumber}" +
                      (string.IsNullOrEmpty(statusQuery) ? "" : $"&{statusQuery}") +
                      (filter.North.HasValue ? $"&north={filter.North}" : "") +
                      (filter.South.HasValue ? $"&south={filter.South}" : "") +
                      (filter.East.HasValue ? $"&east={filter.East}" : "") +
                      (filter.West.HasValue ? $"&west={filter.West}" : "")
            });
        }

    }
}