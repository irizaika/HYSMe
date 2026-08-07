using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using PetsApi.Data;
using PetsApi.Models;

namespace PetApi.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public string DbName { get; } = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            //builder.ConfigureAppConfiguration((context, config) =>
            //{
            //    config.AddInMemoryCollection(new Dictionary<string, string?>
            //    {
            //        ["Jwt:Secret"] = "ThisIsAVeryStrongAndLongSecretKeyForHmacSha256!!!",
            //        ["Jwt:Issuer"] = "hysme-auth-api",
            //        ["Jwt:Audience"] = "hysme-clien"
            //    });
            //});
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile(
                    "appsettings.Testing.json",
                    optional: false);
            });

            builder.ConfigureTestServices(services =>
            {
                //  Remove EVERYTHING related to DbContext
                var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IDbContextOptionsConfiguration<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Re-register DbContext with InMemory
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(DbName);
                });
            });
        }
    }
}