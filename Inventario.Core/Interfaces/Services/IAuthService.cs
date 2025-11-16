using Inventario.Core.DTOs.Responses;
using Inventario.Core.Utils;

namespace Inventario.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(string? email, string? senha);
        Task LogoutAsync(long? UsuarioId);
    }
}