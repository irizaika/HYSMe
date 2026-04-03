
using Contracts.Models;

namespace AuthApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegistrationResponseDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
    }
}
