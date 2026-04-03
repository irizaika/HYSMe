using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PetsApi.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
        {
            var settingsSection = builder.Configuration.GetSection("ApiSettings");

            var secret = settingsSection.GetValue<string>("Secret");
            var issuer = settingsSection.GetValue<string>("Issuer");
            var audience = settingsSection.GetValue<string>("Audience");

            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("JWT Secret is not configured");
            }

            //var key = Encoding.ASCII.GetBytes(secret);
            var key = Encoding.UTF8.GetBytes(secret);


            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.MapInboundClaims = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    NameClaimType = "name",
                    RoleClaimType = "role"
                };

                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Auth failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });

            return builder;
        }
    }
}
