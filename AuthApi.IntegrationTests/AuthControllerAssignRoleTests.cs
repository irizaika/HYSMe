using Contracts.Models;
using System.Net.Http.Json;


namespace AuthApi.IntegrationTests
{
    public class AuthControllerAssignRoleTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public AuthControllerAssignRoleTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
        }

        //Assign role - success
        [Fact]
        public async Task AssignRole_ShouldReturnOk()
        {
            var email = "role@test.com";

            await _client.PostAsJsonAsync("/api/auth/register", new
            {
                Email = email,
                Password = "Password123!",
                Name = "User"
            }, cancellationToken: TestContext.Current.CancellationToken);

            var response = await _client.PostAsJsonAsync("/api/auth/assign-role", new
            {
                Email = email,
                Role = "Admin"
            }, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<string>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.True(content.IsSuccess);
            Assert.Equal("Role assigned", content.Message);
        }

        //Assign role - invalid request, empty email and role
        [Fact]
        public async Task AssignRole_InvalidRequest_ShouldReturnBadRequest()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/assign-role", new
            {
                Email = "",
                Role = ""
            }, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //Assign role - user not found
        [Fact]
        public async Task AssignRole_UserNotFound_ShouldReturnNotFound()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/assign-role", new
            {
                Email = "missing@test.com",
                Role = "Admin"
            }, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        //Assign role - invalid request, empty email
        [Fact]
        public async Task AssignRole_InvalidRequest_ShouldFail()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/assign-role", new
            {
                Email = (string)null,
                Role = "Admin"
            }, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}