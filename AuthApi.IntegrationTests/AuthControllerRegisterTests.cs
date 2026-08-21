using AuthApi.Data;
using AuthApi.Models;
using Contracts.Models;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;

namespace AuthApi.IntegrationTests
{
    public class AuthControllerRegisterTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public AuthControllerRegisterTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
        }

        //Register - success
        [Fact]
        public async Task Register_ShouldReturnSuccess()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/register", new
            {
                Email = "test@test.com",
                Password = "Password123!",
                Name = "Test"
            }, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.True(content.IsSuccess);
            Assert.Null(content.Errors);
            Assert.Equal("User registered successfully", content.Message);
        }

        // Register - email exists
        [Fact]
        public async Task Register_EmailExists_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var result = await userManager.CreateAsync(
                new ApplicationUser
                {
                    Email = "exists@test.com",
                    UserName = "exists@test.com",
                    Name = "Existing User"
                },
                "Password123!");

            Assert.True(result.Succeeded);

            var response = await _client.PostAsJsonAsync("/api/auth/register", new
            {
                Email = "exists@test.com",
                Password = "Password123!",
                Name = "Test"
            }, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.False(content.IsSuccess); 
            Assert.Null(content.Data);
            Assert.NotNull(content.Errors);
        }

        // Register - success with same name, but different email
        [Fact]
        public async Task Register_NameExists_ShouldNotReturnError()
        {
            // seed database with an existing user
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.ApplicationUsers.Add(new ApplicationUser
                {
                    UserName = "newEmail@test.com",
                    Email = "newEmail@test.com",
                    Name = "Test User" //same name but different email
                });

                db.SaveChanges();
            }

            var response = await _client.PostAsJsonAsync("/api/auth/register", new
            {
                Email = "email@test.com",
                Password = "Password123!",
                Name = "Test User"
            }, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.True(content.IsSuccess);
            Assert.Null(content.Errors);
            Assert.Equal("User registered successfully", content.Message);
        }
       
        [Fact]
        public async Task Register_InvalidPassword_ShouldReturnError()
        {
            var request = new
            {
                Email = "test2@test.com",
                Password = "123", // too weak
                Name = "Test"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", request, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.False(content.IsSuccess); // API wraps errors as success response
            Assert.Null(content.Data);
            Assert.NotNull(content.Errors);
            Assert.NotEmpty(content.Errors);
            Assert.Contains(content.Errors, e => e.Field == "Password");
        }

        [Fact]
        public async Task Register_MissingEmail_ShouldFail()
        {
            var request = new
            {
                Email = "",
                Password = "Password123!",
                Name = "Test"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", request, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.False(content.IsSuccess);
            Assert.Null(content.Data);
            Assert.NotNull(content.Errors);
            Assert.NotEmpty(content.Errors);
            Assert.Contains(content.Errors, e => e.Field == "Email");
        }

        [Fact]
        public async Task Register_ShouldPersistUserInDatabase()
        {
            var email = "persist@test.com";

            var request = new
            {
                Email = email,
                Password = "Password123!",
                Name = "Persist User"
            };

            await _client.PostAsJsonAsync("/api/auth/register", request, cancellationToken: TestContext.Current.CancellationToken);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = db.ApplicationUsers.FirstOrDefault(u => u.Email == email);

            Assert.NotNull(user);
            Assert.Equal("Persist User", user.Name);
        }
      
        [Fact]
        public async Task Register_ShouldBeCaseInsensitive_ForEmail()
        {
            await _client.PostAsJsonAsync("/api/auth/register", new
            {
                Email = "case@test.com",
                Password = "Password123!",
                Name = "User"
            }, cancellationToken: TestContext.Current.CancellationToken);

            var response = await _client.PostAsJsonAsync("/api/auth/register", new
            {
                Email = "CASE@test.com",
                Password = "Password123!",
                Name = "User"
            }, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);


            var content = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.False(content.IsSuccess);
            Assert.Null(content.Data);
            Assert.NotNull(content.Errors);
            Assert.NotEmpty(content.Errors);
            Assert.Contains(content.Errors, e => e.Field == "Email"); // email already taken
        }

    }
}
