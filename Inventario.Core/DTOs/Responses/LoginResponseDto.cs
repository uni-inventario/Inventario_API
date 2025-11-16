namespace Inventario.Core.DTOs.Responses
{
    public class LoginResponseDto
    {
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public LoginResponseDto(string token, DateTime expiresAt)
        {
            Token = token;
            ExpiresAt = expiresAt;
        }
    }
}