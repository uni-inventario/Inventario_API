using Inventario.Core.DTOs.Requests;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.Utils;

namespace Inventario.Core.Interfaces.Handlers
{
    public interface IEstoqueHandler
    {
        Task<ApiResponse<List<EstoqueResponseDto>>> GetAllAsync(long usuarioId);
        Task<ApiResponse<EstoqueResponseDto>> GetByIdAsync(long id, long usuarioId);
        Task<ApiResponse<EstoqueResponseDto>> AddAsync(EstoqueRequestDto? entity, long usuarioId);
        Task<ApiResponse<EstoqueResponseDto>> UpdateAsync(EstoqueRequestDto? entity, long usuarioId);
        Task<ApiResponse<EstoqueResponseDto>> DeleteAsync(long id, long usuarioId);
    }
}