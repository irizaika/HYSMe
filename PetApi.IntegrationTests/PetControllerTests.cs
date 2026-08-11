
using System.Net.Http.Json;
using Contracts.Models;
using PetsApi.Data;
using PetsApi.Models;


namespace PetApi.IntegrationTests
{
    public class PetControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly HttpClient _authenticatedClient;
        private readonly CustomWebApplicationFactory _factory;

        public PetControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;

            ResetDatabase(); // called for every [Fact]

            _client = factory.CreateClient();

            _authenticatedClient = CreateAuthenticatedClient();
        }

        private void ResetDatabase()
        {
            using var scope = _factory.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Pets.RemoveRange(db.Pets);
            db.Sightings.RemoveRange(db.Sightings);

            db.SaveChanges();
        }

        private HttpClient CreateAuthenticatedClient()
        {
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    JwtHelper.CreateJwt());

            return _client;
        }


        [Fact]
        public async Task GetPet_ShouldReturnPet()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var pet = new Pet
            {
                Name = "Rikky",
                Type = "Dog",
                UserId = "user1"
            };

            db.Pets.Add(pet);
            db.SaveChanges();

            var response = await _client.GetAsync($"/api/pets/{pet.Id}", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content =
                await response.Content.ReadFromJsonAsync<ApiResponse<PetDto>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.True(content.IsSuccess);
            Assert.NotNull(content.Data);
            Assert.Equal("Rikky", content.Data.Name);
        }

        [Fact]
        public async Task CreatePet_ShouldPersistPet()
        {
            var request = new PetDto
            {
                Name = "Bella",
                Type = "Dog",
                Breed = "Labrador",
                Color = "Black",
                Latitude = 51.5,
                Longitude = -0.1,
                Status =  Contracts.Enums.PetStatus.Lost,
                Description = "Friendly dog",
            };

            var response =
                await _authenticatedClient.PostAsJsonAsync("/api/pets", request, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var pet = db.Pets.FirstOrDefault(x => x.Name == "Bella");

            Assert.NotNull(pet);
            Assert.Equal("Dog", pet.Type);
            Assert.Equal("Labrador", pet.Breed);
            Assert.Equal("Black", pet.Color);
        }

        [Fact]
        public async Task CreatePet_ShouldSaveAllFields()
        {
            var dto = new PetDto
            {
                Name = "Max",
                Type = "Dog",
                Breed = "Beagle",
                Color = "White",
                Description = "Friendly",
                Latitude = 51.501,
                Longitude = -0.101,
                LastSeenAddress = "London",
                ImageUrl = "/images/max.jpg"
            };

            await _authenticatedClient.PostAsJsonAsync("/api/pets", dto, cancellationToken: TestContext.Current.CancellationToken);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var pet = db.Pets.Single(x => x.Name == "Max");

            Assert.Equal(dto.Type, pet.Type);
            Assert.Equal(dto.Breed, pet.Breed);
            Assert.Equal(dto.Color, pet.Color);
            Assert.Equal(dto.Description, pet.Description);
            Assert.Equal(dto.LastSeenAddress, pet.LastSeenAddress);
            Assert.Equal(dto.ImageUrl, pet.ImageUrl);
            Assert.Equal(dto.Latitude, pet.Latitude);
            Assert.Equal(dto.Longitude, pet.Longitude);
        }

        [Fact]
        public async Task QueryPets_ShouldSearchByName()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Pets.AddRange(
                new Pet { Name = "Rikky" },
                new Pet { Name = "Bella" });

            db.SaveChanges();

            var response =
                await _client.GetAsync("/api/pets/query?search=rik", TestContext.Current.CancellationToken);

            var content =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<PagedResult<PetDto>>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.NotNull(content.Data);
            Assert.Single(content.Data.Items);
            Assert.Equal("Rikky", content.Data.Items[0].Name);
        }

        [Fact]
        public async Task QueryPets_ShouldSearchAllWords()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Pets.Add(new Pet
            {
                Name = "Rikky",
                Color = "White",
                LastSeenAddress = "London"
            });

            db.Pets.Add(new Pet
            {
                Name = "Rikky",
                Color = "White",
                LastSeenAddress = "Leeds"
            });

            db.SaveChanges();

            var response =
                await _client.GetAsync("/api/pets/query?search=london rikky white", TestContext.Current.CancellationToken);

            var content =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<PagedResult<PetDto>>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.NotNull(content.Data);
            Assert.Single(content.Data.Items);
        }

        [Fact]
        public async Task QueryPets_ShouldPaginate()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            for (int i = 0; i < 15; i++)
            {
                db.Pets.Add(new Pet
                {
                    Name = $"Pet{i}"
                });
            }

            db.SaveChanges();

            var response =
                await _client.GetAsync("/api/pets/query?pageNumber=2&itemPerPage=10", TestContext.Current.CancellationToken);

            var content =
                await response.Content.ReadFromJsonAsync<
                    ApiResponse<PagedResult<PetDto>>>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(content);
            Assert.NotNull(content.Data);
            Assert.Equal(5, content.Data.Items.Count);

           int? list = null;
            //Console.WriteLine(list);
        }

    }
}
