using AuthApi.Services.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var result = await _authService.Register(model);

            if (result != null && result.Count != 0) // result is error or null
            {
                return BadRequest(ApiResponse<object>.Fail(result));
            }

            return Ok(ApiResponse<object>.Success(message: "User registered successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var result = await _authService.Login(model);

            if (result.User == null)
            {
                return Unauthorized(ApiResponse<List<Error>>.Fail([ 
                    result.Error ??
                    new Error { Field = "Password/UserName", Message = "Username or password is incorrect" } 
                ], "Unauthorized"));
            }

            return Ok(ApiResponse<LoginResponseDto>.Success(result));
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Role))
            {
                return BadRequest(ApiResponse<List<Error>>.Fail([ 
                    new() { Field = "Email/Role", Message = "Email and Role are required" } 
                ], "Invalid request"));
            }

            var success = await _authService.AssignRole(model.Email, model.Role.ToUpper());

            if (!success)
            {
                return NotFound(ApiResponse<List<Error>>.Fail([ 
                    new Error { Field = "Email", Message = "User not found" } 
                ], "User not found"));
            }

            return Ok(ApiResponse<string>.Success(null, "Role assigned"));
        }
    }
}
