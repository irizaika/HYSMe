using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Services.Interfaces;
using AuthApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//using Microsoft.OpenPetsApi.Models;

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApiDocument(); //nswag generate openapi spec

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
ApplyMigrations(app);
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
