using Contracts.Enums;
using Contracts.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Services;
using WebApp.Services.Interfaces;
using WebApp.Utility;

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
            ResponseDto? responseDto = await _authService.LoginAsync(login);

            if (responseDto != null && responseDto.IsSuccess)
            {
   
                string response = Convert.ToString(responseDto.Result) ?? string.Empty;

                LoginResponseDto? loginResponseDto =
                    JsonConvert.DeserializeObject<LoginResponseDto>(response);

                if (loginResponseDto != null)
                {
                    await SignInUser(loginResponseDto);
                    _tokenProvider.SetToken(loginResponseDto.Token);

                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    //TempData["error"] = responseDto?.Message ?? "Login failed";
                    ModelState.AddModelError(loginResponseDto?.Error?.Field??"", loginResponseDto?.Error?.Message?? "Login failed");
                    return View(obj);
                }
           
            }
            else
            {

                string response = Convert.ToString(responseDto?.Result) ?? "[]";
                //Error? error = JsonConvert.DeserializeObject<Error>(response);
                Error? error = JsonConvert.DeserializeObject<Error>(response);
                ModelState.AddModelError(error?.Field ?? "", error?.Message ?? "Login failed");

                return View(obj);
            }
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

            ResponseDto? result = await _authService.RegisterAsync(register);
            ResponseDto? assignRole;

            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = Role.RoleUser;
                }
                assignRole = await _authService.AssignRoleAsync(register);
                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registertion succesful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                string response = Convert.ToString(result?.Result) ?? "[]";
                RegistrationResponseDto? errorResponse = JsonConvert.DeserializeObject<RegistrationResponseDto>(response);

                 if (errorResponse?.Errors != null && errorResponse.Errors.Any())
                {
                    var groupedErrors = errorResponse.Errors
                        .GroupBy(e => e.Field ?? string.Empty);

                    foreach (var group in groupedErrors)
                    {
                        var combinedMessage = string.Join("\n", 
                            group.Select(e => e.Message ?? "Registration failed"));

                        ModelState.AddModelError(group.Key, combinedMessage);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Registration failed");
                }

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

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
