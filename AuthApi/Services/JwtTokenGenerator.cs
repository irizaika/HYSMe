using AuthApi.Models;
using AuthApi.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthApi.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions;
        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions) => _jwtOptions = jwtOptions.Value;

        public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                //var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
                var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);
                
                var claimList = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Email, applicationUser.Email ?? ""),
                    new(JwtRegisteredClaimNames.Sub, applicationUser.Id ?? ""),
                    new(ClaimTypes.NameIdentifier, applicationUser.Id ?? ""),
                    new(JwtRegisteredClaimNames.Name, applicationUser.UserName ?? "")
                };

                claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Audience = _jwtOptions.Audience,
                    Issuer = _jwtOptions.Issuer,
                    Subject = new ClaimsIdentity(claimList),
                    Expires = DateTime.UtcNow.AddDays(7), // to do 
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating token: {ex.Message}");
                throw new Exception("Token generation failed", ex);
              //  return ""; // what to return 
                //throw;
            }
        }
    }
}
