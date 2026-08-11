using AuthApi.Models;
using AuthApi.Services;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace AuthApi.UnitTests
{
    public class JwtTokenGeneratorTests
    {
        private readonly JwtTokenGenerator _generator;

        public JwtTokenGeneratorTests()
        {
            var options = Options.Create(new JwtOptions
            {
                Secret = "supersecretkeysupersecretkey1234", // must be long enough 32 char
                Issuer = "test-issuer",
                Audience = "test-audience"
            });

            _generator = new JwtTokenGenerator(options);
        }

        [Fact]
        public void GenerateToken_ShouldReturnToken()
        {
            var user = GetUser();

            var token = _generator.GenerateToken(user, new List<string>());

            Assert.False(string.IsNullOrEmpty(token));

            //token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GenerateToken_ShouldContainCorrectClaims()
        {
            var user = GetUser();

            var token = _generator.GenerateToken(user, new List<string>());

            var jwt = ReadToken(token);

            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id);
            //Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id); // not there
            Assert.Contains(jwt.Claims, c => c.Type == "nameid" && c.Value == user.Id); // it there as NameId
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Name && c.Value == user.UserName);
        }

        [Fact]
        public void GenerateToken_ShouldContainRoles()
        {
            var user = GetUser();
            var roles = new List<string> { "Admin", "User" };

            var token = _generator.GenerateToken(user, roles);

            var jwt = ReadToken(token);

            // Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            // Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
            Assert.Contains(jwt.Claims, c => c.Type == "role" && c.Value == "Admin");
            Assert.Contains(jwt.Claims, c => c.Type == "role" && c.Value == "User");
        }

        [Fact]
        public void GenerateToken_ShouldSetIssuerAndAudience()
        {
            var user = GetUser();

            var token = _generator.GenerateToken(user, new List<string>());

            var jwt = ReadToken(token);

            Assert.Equal("test-issuer", jwt.Issuer);
            Assert.Contains("test-audience", jwt.Audiences);
        }

        [Fact]
        public void GenerateToken_ShouldSetExpiration()
        {
            var user = GetUser();

            var token = _generator.GenerateToken(user, new List<string>());

            var jwt = ReadToken(token);

            Assert.True(jwt.ValidTo > DateTime.UtcNow);
        }

            [Fact]
        public void GenerateToken_ShouldHandleNullFields()
        {
            var user = new ApplicationUser
            {
                Id = null,
                Email = null,
                UserName = null
            };

            var token = _generator.GenerateToken(user, new List<string>());

            var jwt = ReadToken(token);

            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email);
        }

        [Fact]
        public void GenerateToken_ShouldThrow_WhenSecretIsInvalid()
        {
            var badOptions = Options.Create(new JwtOptions
            {
                Secret = "", // invalid
                Issuer = "test",
                Audience = "test"
            });

            var generator = new JwtTokenGenerator(badOptions);

            var user = GetUser();

            var exception = Assert.Throws<Exception>(
                () => generator.GenerateToken(user, new List<string>())
               );

            Assert.Equal("Token generation failed", exception.Message);
        }

        // -------- HELPERS --------
        private ApplicationUser GetUser()
        {
            return new ApplicationUser
            {
                Id = "1",
                Email = "test@test.com",
                UserName = "test@test.com"
            };
        }

        private JwtSecurityToken ReadToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }
    }
}