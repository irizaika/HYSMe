using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PetApi.IntegrationTests
{
    public static class JwtHelper
    {
        public static string CreateJwt()
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "ThisIsAVeryStrongAndLongSecretKeyForHmacSha256!!!"));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "hysme-auth-api",
                audience: "hysme-client",
                claims:
                [
                    new Claim("sub", "test-user"),
                    new Claim("name", "Test User"),
                    new Claim("role", "USER")
                ],
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
