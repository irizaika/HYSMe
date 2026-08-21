using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Contracts.Models;

namespace AuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(roleName));
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            return result.Succeeded;

        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            if (string.IsNullOrWhiteSpace(loginRequestDto?.UserName))
            {
                return FailedLoginResponse("Username is required",nameof(loginRequestDto.UserName));
            }

            if (string.IsNullOrWhiteSpace(loginRequestDto?.Password))
            {
                return FailedLoginResponse("Password is required", nameof(loginRequestDto.Password));
            }

            var normalizedUserName = _userManager.NormalizeName(loginRequestDto.UserName);

            var user = await _userManager.FindByNameAsync(normalizedUserName);

            if (user == null)
            {
                return FailedLoginResponse("User does not exist", nameof(loginRequestDto.UserName));
            }

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (user == null || isValid == false)
            {
                return FailedLoginResponse("Invalid password", nameof(loginRequestDto.Password));
            }

            //if user was found, Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDTO = new()
            {
                Email = user.Email ?? "",
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber ?? ""
            };

            LoginResponseDto loginResponseDto = new()
            {
                User = userDTO,
                Token = token
            };

            return loginResponseDto;
        }

        public async Task<List<Error>?> Register(RegistrationRequestDto registrationRequestDto)
        {
            var email = await _userManager.FindByEmailAsync(registrationRequestDto.Email);


            if (email != null)
            {
                return [new() {Message = "Email already registered", Field = "Email"}];
            }

            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                   // var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = user.Email ?? "",
                        ID = user.Id,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber ?? ""
                    };

                    return null;
                }
                else
                {
                    if (result == null)
                    {
                        return [new() { Message = "Registration failed", Field = "" }];
                    }
                    var errors = result.Errors.Select(e => new Error
                    {
                        Message = e.Description,
                        Field = MapErrorToField(e.Code)
                    }).ToList();

                    return errors;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); //to do logging
            }
            
            return [new() { Message = "Error Encountered", Field = "" }];
        }


        private static string MapErrorToField(string code)
        {
            if (code.Contains("Password")) return "Password";
            //    if (code.Contains("Email") || code.Contains("UserName")) return "Email";

            return "Email";
        }

        private static LoginResponseDto FailedLoginResponse(string message, string field)
        {
            return new LoginResponseDto()
            {
                User = null,
                Token = "",
                Error = new Error
                {
                    Message = message,
                    Field = field
                }
            };
        }
    }
}
