using Contracts.Enums;
using Contracts.Models;
using WebApp.Services.Interfaces;
using WebApp.Utility;

namespace WebApp.Services
{
    public class PetSightingService : IPetSightingService
    {
        private readonly IBaseService _baseService;
        public PetSightingService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        //public async Task<ResponseDto?> GetAllPetsAsync()
        //{
        //    return await _baseService.SendAsync(new RequestDto
        //    {
        //        ApiType = ApiType.GET,
        //        Url = SD.APIBase + "/api/pets"
        //    });
        //}

        //public async Task<ResponseDto?> GetPetByIdAsync(int id)
        //{
        //    return await _baseService.SendAsync(new RequestDto
        //    {
        //        ApiType = ApiType.GET,
        //        Url = SD.APIBase + $"/api/pets/{id}"
        //    });
        //}

        public async Task<ResponseDto?> CreatePetSightingAsync(SightingDto dto)
        {
            ResponseDto? responce;
            try
            {
                responce = await _baseService.SendAsync(new RequestDto
                {
                    ApiType = ApiType.POST,
                    Data = dto,
                    Url = SD.APIBase + "/api/sightings"
                });

                return responce;
            }
            catch
            {
                return new ResponseDto()
                {
                    IsSuccess = false,
                    Message = "Something went wrong while creating the record. Please try again later.",
                    Result = null
                };
            }
        }

        //public async Task<ResponseDto?> UpdatePetAsync(int id, PetDto petDto)
        //{
        //    return await _baseService.SendAsync(new RequestDto
        //    {
        //        ApiType = ApiType.PUT,
        //        Data = petDto,
        //        Url = SD.APIBase + $"/api/pets/{id}"
        //    });
        //}

        //public async Task<ResponseDto?> GetPetsInArea(AreaDto area)
        //{
        //    return await _baseService.SendAsync(new RequestDto
        //    {
        //        ApiType = ApiType.GET,
        //        Url = $"{SD.APIBase}/api/pets/area?north={area.North}&south={area.South}&east={area.East}&west={area.West}"
        //    });
        //}

        //public async Task<ResponseDto?> QueryPetsAsync(PetQueryDto filter)
        //{
        //    string statusQuery = "";
        //    if (filter.Statuses != null && filter.Statuses.Any())
        //    {
        //        statusQuery = string.Join("&", filter.Statuses.Select(s => $"statuses={s}"));
        //    }

        //    return await _baseService.SendAsync(new RequestDto
        //    {
        //        ApiType = ApiType.GET,
        //        Url = $"{SD.APIBase}/api/pets/query?" +
        //              $"itemPerPage={filter.ItemPerPage}&pageNumber={filter.PageNumber}" +
        //              (string.IsNullOrEmpty(statusQuery) ? "" : $"&{statusQuery}") +
        //              (filter.North.HasValue ? $"&north={filter.North}" : "") +
        //              (filter.South.HasValue ? $"&south={filter.South}" : "") +
        //              (filter.East.HasValue ? $"&east={filter.East}" : "") +
        //              (filter.West.HasValue ? $"&west={filter.West}" : "")
        //    });
        //}
    }
}