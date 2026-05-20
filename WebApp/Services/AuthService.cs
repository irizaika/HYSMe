using Contracts.Models;
using WebApp.Utility;
using Contracts.Enums;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ApiResponse<RegistrationResponseDto>?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync<RegistrationResponseDto>(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/assign-role"
            });
        }

        public async Task<ApiResponse<LoginResponseDto>?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var retVal =  await _baseService.SendAsync<LoginResponseDto>(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            }, withBearer: false);

            return retVal;
        }

        public async Task<ApiResponse<object>?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync<object>(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            }, withBearer: false);
        }
    }
}
