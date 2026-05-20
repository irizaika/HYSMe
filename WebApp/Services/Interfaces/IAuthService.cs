using Contracts.Models;

namespace WebApp.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponseDto>?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ApiResponse<object>?> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ApiResponse<RegistrationResponseDto>?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto);
    }
}
