namespace Contracts.Models
{
    public class LoginResponseDto
    {
        public UserDto? User { get; set; }
        public string Token { get; set; } = string.Empty;
        public Error Error { get; set; } = new();
    }
}
