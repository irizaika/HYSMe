
using Contracts.Models;

namespace AuthApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<List<Error>?> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
    }
}
