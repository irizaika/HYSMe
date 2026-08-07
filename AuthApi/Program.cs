using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Services;
using AuthApi.Services.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();


// Add services to the container.
//builder.Services.AddControllers();
//catch ValidationErrorResponse here ane return ApiResponse<object> with errors (for example BadRequest Register_MissingEmail_ShouldFail)
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors.Select(e => new Error
                {
                    Field = x.Key,
                    Message = e.ErrorMessage
                }))
                .ToList();
            return new BadRequestObjectResult(
                ApiResponse<object>.Fail(errors, "Validation failed")
            );
        };
    })
    .AddXmlSerializerFormatters();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string[] strings = ["http://localhost:5173",]; // todo config file

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowed = strings;

        policy.WithOrigins([.. allowed.Where(x => !string.IsNullOrEmpty(x))])
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

//app.UseHttpsRedirection();
app.UseAuthentication();


app.UseAuthorization();

app.MapControllers();



if (!app.Environment.IsEnvironment("Testing"))
{
    ApplyMigrations(app);
}

app.Run();

static void ApplyMigrations(IApplicationBuilder app)
{

        using var scope = app.ApplicationServices.CreateScope();
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (_db.Database.GetPendingMigrations().Any())
        {
            _db.Database.Migrate();
        }

}
