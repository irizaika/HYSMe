using AuthApi.Models;

namespace AuthApi.Services.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
