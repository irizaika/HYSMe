using Contracts.Enums;
using Contracts.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Services;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel loginRequestDto = new();
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel obj)
        {
            var login = MapperHelper.MapToDto(obj);

            var response = await _authService.LoginAsync(login);

            if (response != null && response.IsSuccess && response.Data != null)
            {
                var loginDto = response.Data;

                await SignInUser(loginDto);
                _tokenProvider.SetToken(loginDto.Token);

                return RedirectToAction("Index", "Home");
            }

            if (response?.Errors != null && response.Errors.Count > 0)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(error.Field ?? "", error.Message ?? "Login failed");
                }
            }
            else
            {
                ModelState.AddModelError("", response?.Message ?? "Login failed");
            }

            return View(obj);
        }


        [HttpGet]
        public IActionResult Register()
        {
            //var roleList = new List<SelectListItem>()
            //{
            //    new() {Text=Role.RoleUser, Value = Role.RoleUser},
            //    new() {Text=Role.RoleAdmin, Value = Role.RoleAdmin}
            //};

            //ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel obj)
        {
            var register = MapperHelper.MapToDto(obj);

            ApiResponse<object>? result = await _authService.RegisterAsync(register);
            ApiResponse<RegistrationResponseDto>? assignRole;

            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = Role.RoleUser;
                }
                assignRole = await _authService.AssignRoleAsync(register);
                //if (assignRole != null && assignRole.IsSuccess)
                //{
                //    TempData["success"] = "Registration successful";
                //    return RedirectToAction(nameof(Login));
                //}
                if (assignRole != null && assignRole.IsSuccess)
                {
                    // auto login
                    var loginResponse = await _authService.LoginAsync(new LoginRequestDto
                    {
                        UserName = register.Email,
                        Password = register.Password
                    });
                    
                    if (loginResponse != null && loginResponse.IsSuccess && loginResponse.Data != null)
                    { 
                        await SignInUser(loginResponse.Data);
                        _tokenProvider.SetToken(loginResponse.Data.Token);

                        TempData["success"] = "Welcome!";
                        return RedirectToAction("Index", "Home");
                    }
                }

            }
            else
            {
                //if (result != null && result.Errors != null && result.Errors.Any())
                //{
                //    var groupedErrors = result.Errors
                //        .GroupBy(e => e.Field ?? string.Empty);

                //    foreach (var group in groupedErrors)
                //    {
                //        var combinedMessage = string.Join("\n", 
                //            group.Select(e => e.Message ?? "Registration failed"));

                //        ModelState.AddModelError(group.Key, combinedMessage);
                //    }
                //}
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Registration failed");
                //}

                if (result?.Errors != null && result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        //ModelState.AddModelError(error.Field ?? "", error.Message ?? "Registration failed");
                        var groupedErrors = result.Errors
                            .GroupBy(e => e.Field ?? string.Empty);

                        foreach (var group in groupedErrors)
                        {
                            var combinedMessage = string.Join("\n",
                                group.Select(e => e.Message ?? "Registration failed"));

                            ModelState.AddModelError(group.Key, combinedMessage);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", result?.Message ?? "Registration failed");
                }

                TempData["error"] = result != null && result.Message != null && result.Message.Length > 0? result.Message : "Registration failed";


                // ModelState.AddModelError(error?.Field ?? "Password", error?.Message ?? "Registration failed");
                //  TempData["error"] = result!=null ? result.Message : "Registration failed";
            }
            //var roleList = new List<SelectListItem>()
            //{
            //    new() {Text=Role.RoleUser, Value = Role.RoleUser},
            //    new() {Text=Role.RoleAdmin, Value = Role.RoleAdmin}
            //};

            //ViewBag.RoleList = roleList;

            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index","Home");
        }


        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            var emailClaim = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email);
            var subClaim = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub);
            var nameClaim = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name);
            var idClaim = jwt.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);
            var roleClaim = jwt.Claims.FirstOrDefault(u => u.Type == "role");

            if (emailClaim != null)
            {
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, emailClaim.Value));
                identity.AddClaim(new Claim(ClaimTypes.Name, emailClaim.Value));
            }
            if (subClaim != null)
            {
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, subClaim.Value));
            }
            if (nameClaim != null)
            {
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, nameClaim.Value));
            }
            if (roleClaim != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
            }
            if (idClaim != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, idClaim.Value));
            }

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
