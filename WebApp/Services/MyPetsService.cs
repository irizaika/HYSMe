using Contracts.Enums;
using Contracts.Models;
using WebApp.Services.Interfaces;
using WebApp.Utility;

namespace WebApp.Services
{
    public class MyPetsService : IMyPetsService
    {
        private readonly IBaseService _baseService;

        public MyPetsService(IBaseService baseService)
        {
            _baseService = baseService;
        }
    
        public async Task<ApiResponse<List<PetDto>>?> GetMyPetsAsync()
        {
            return await _baseService.SendAsync<List<PetDto>>(
                new RequestDto
                {
                    ApiType = ApiType.GET,
                    Url = SD.APIBase + "/api/mypets/my-pets"
                });
        }

        public async Task<ApiResponse<List<SightingDto>>?> GetMySightingsAsync()
        {
            return await _baseService.SendAsync<List<SightingDto>>(
                new RequestDto
                {
                    ApiType = ApiType.GET,
                    Url = SD.APIBase + "/api/mypets/my-sightings"
                });
        }
    }
}
