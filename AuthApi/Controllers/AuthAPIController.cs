using Contracts.Models;
using AuthApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        //private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;
        public AuthAPIController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;

            _response = new();
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var error = await _authService.Register(model);
            if (error != null && error.Errors.Count > 0)
            {
                _response.IsSuccess = false;
                _response.Message = "";
                _response.Result = error;
                return Ok(_response);
            }
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                _response.Result = loginResponse.Error;
                return Ok(_response); // Erro in BadRequest will be parsed as ValidationErrorResponse
            }
            _response.Result = loginResponse;
            return Ok(_response);

        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            if (model.Email != null && model.Role != null)
            {
                var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
                if (!assignRoleSuccessful)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error encountered";
                    return BadRequest(_response);
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered";
                return BadRequest(_response);

            }

            return Ok(_response);

        }


    }
}
