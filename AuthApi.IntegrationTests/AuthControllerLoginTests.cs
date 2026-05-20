using AuthApi.Data;
using AuthApi.Models;
using Contracts.Models;
using System.Net.Http.Json;


namespace AuthApi.IntegrationTests
{
    public class AuthControllerLoginTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public AuthControllerLoginTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
        }

        // Login - success
        [Fact]
        public async Task Login_ShouldReturnToken_WhenValid()
        {
            var email = "login@test.com";

            await _client.PostAsJsonAsync("/api/auth/register", new
            {
                Email = email,
                Password = "Password123!",
                Name = "User"
            }, cancellationToken: TestContext.Current.CancellationToken);

            var response = await _client.PostAsJsonAsync("/api/auth/login", new
            {
                UserName = email,
                Password = "Password123!"
            }, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.True(content.IsSuccess);
            Assert.NotNull(content.Data);
            Assert.Null(content.Errors);
            Assert.NotNull(content.Data.Token);
        }

        // Login - wrong password
        [Fact]
        public async Task Login_WrongPassword_ShouldReturnUnauthorized()
        {
            var email = "wrong@test.com";

            await _client.PostAsJsonAsync("/api/auth/register", new
            {
                Email = email,
                Password = "Password123!",
                Name = "User"
            }, cancellationToken: TestContext.Current.CancellationToken);

            var response = await _client.PostAsJsonAsync("/api/auth/login", new
            {
                UserName = email,
                Password = "WrongPassword"
            }, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<Error>>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.False(content.IsSuccess);
            Assert.Null(content.Data);
            Assert.NotNull(content.Errors);
            Assert.Equal("Password", content.Errors[0].Field);
        }

        //Login - user not found
        [Fact]
        public async Task Login_UserDoesNotExist_ShouldReturnUnauthorized()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login", new
            {
                UserName = "nouser@test.com",
                Password = "Password123!"
            }, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<Error>>>(cancellationToken: TestContext.Current.CancellationToken);


            Assert.NotNull(content);
            Assert.False(content.IsSuccess);
            Assert.Null(content.Data);
            Assert.NotNull(content.Errors);
            Assert.Equal("UserName", content.Errors[0].Field); //todo should be user name not exists
        }
    }
}