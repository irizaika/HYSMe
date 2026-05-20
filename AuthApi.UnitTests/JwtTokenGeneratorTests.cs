using AuthApi.Models;
using AuthApi.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GenerateToken_ShouldContainCorrectClaims()
        {
            var user = GetUser();

            var token = _generator.GenerateToken(user, new List<string>());

            var jwt = ReadToken(token);

            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id);
            //jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id); // not there
            jwt.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == user.Id); // it there as NameId
            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Name && c.Value == user.UserName);
        }

        [Fact]
        public void GenerateToken_ShouldContainRoles()
        {
            var user = GetUser();
            var roles = new List<string> { "Admin", "User" };

            var token = _generator.GenerateToken(user, roles);

            var jwt = ReadToken(token);

            //jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            //jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
            jwt.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Admin");
            jwt.Claims.Should().Contain(c => c.Type == "role" && c.Value == "User");
        }

        [Fact]
        public void GenerateToken_ShouldSetIssuerAndAudience()
        {
            var user = GetUser();

            var token = _generator.GenerateToken(user, new List<string>());

            var jwt = ReadToken(token);

            jwt.Issuer.Should().Be("test-issuer");
            jwt.Audiences.Should().Contain("test-audience");
        }

        [Fact]
        public void GenerateToken_ShouldSetExpiration()
        {
            var user = GetUser();

            var token = _generator.GenerateToken(user, new List<string>());

            var jwt = ReadToken(token);

            jwt.ValidTo.Should().BeAfter(DateTime.UtcNow);
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

            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email);
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

            Action act = () => generator.GenerateToken(user, new List<string>());

            act.Should().Throw<Exception>()
                .WithMessage("Token generation failed");
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