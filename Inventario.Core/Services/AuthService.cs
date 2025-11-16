using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Inventario.Core.Interfaces.Services;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.Utils;

namespace Inventario.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public AuthService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(string? email, string? senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                throw new Exception("Email e senha são obrigatórios.");

            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            if (usuario is null)
                throw new Exception("Usuário não encontrado.");

            if (!VerifyPassword(senha, usuario))
                throw new Exception("Senha inválida.");

            var (token, exp) = GenerateJwt(usuario);
            await _usuarioRepository.UpdateTokenAsync(usuario.Id, token);

            return new ApiResponse<LoginResponseDto>(new LoginResponseDto(token, exp));
        }

        public async Task LogoutAsync(long? usuarioId)
        {
            if (usuarioId is null)
                throw new Exception("Informe um usuário válido.");

            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId.Value);
            if (usuario is null)
                throw new Exception("Usuário não encontrado.");

            await _usuarioRepository.UpdateTokenAsync(usuarioId.Value, null);
        }

        private (string token, DateTime expiresAt) GenerateJwt(Usuario user)
        {
            var keyString = Environment.GetEnvironmentVariable("JWT_KEY");
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            var expiresMinutes = Environment.GetEnvironmentVariable("JWT_EXPIRES");

            if (string.IsNullOrWhiteSpace(keyString))
                throw new Exception("JWT_KEY não foi definida no ambiente.");

            if (Encoding.UTF8.GetBytes(keyString).Length < 32)
                throw new Exception("JWT_KEY deve conter pelo menos 32 bytes.");

            var expiration = DateTime.UtcNow.AddMinutes(
                !string.IsNullOrWhiteSpace(expiresMinutes) ? int.Parse(expiresMinutes) : 1440
            );

            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Nome)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            var rawToken = new JwtSecurityTokenHandler().WriteToken(token);

            return (rawToken, expiration);
        }

        private bool VerifyPassword(string senha, Usuario usuario)
        {
            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);
            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
