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
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email != null && u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;

        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName != null && u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            if (user == null)
            {
                return new LoginResponseDto() { User = null, Token = "", Error = new Error { Message = "User does not exist", Field = nameof(loginRequestDto.UserName) } };
            }

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "", Error = new Error { Message = "Invalid password", Field = nameof(loginRequestDto.Password) } };
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

        public async Task<RegistrationResponseDto> Register(RegistrationRequestDto registrationRequestDto)
        {
            var checkIfExists = _db.ApplicationUsers.Any(u => u.UserName == registrationRequestDto.Email);

            if (checkIfExists == true)
            {
                return new RegistrationResponseDto()
                {
                    Errors =
                    [
                        new Error()
                        {
                            Message = "Email alredy registered",
                            Field = "Email"
                        }
                    ]
                };
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
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email ?? "",
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber ?? ""
                    };

                    return new RegistrationResponseDto();
                    //[ 
                    //    new Error()
                    //    {
                    //        Message = "",
                    //        Field = ""
                    //    } 
                    //];
                }
                else
                {
                    if (result == null)
                    {
                        return new RegistrationResponseDto()
                        {
                            Errors =
                            [
                                new Error()
                                {
                                    Message = "Registration failed",
                                    Field = ""
                                }
                            ]
                        };

                    }
                    var errors = result.Errors.Select(e => new Error
                    {
                        Message = e.Description,
                        Field = MapErrorToField(e.Code)
                    }).ToList();

                    return new RegistrationResponseDto() { Errors = errors };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); //to do logging
            }
            return new RegistrationResponseDto()
            {
                Errors = 
                [
                    new Error()
                    {
                        Message = "Error Encountered",
                        Field = ""
                    }
                ]
            };
        }


        private static string MapErrorToField(string code)
        {
            if (code.Contains("Password")) return "Password";
            //    if (code.Contains("Email") || code.Contains("UserName")) return "Email";

            return "Email";
        }
    }
}
