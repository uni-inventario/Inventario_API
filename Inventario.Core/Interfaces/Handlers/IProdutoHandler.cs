using Inventario.Core.DTOs.Requests;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.Utils;

namespace Inventario.Core.Interfaces.Handlers
{
    public interface IProdutoHandler
    {
        Task<ApiResponse<ProdutoResponseDto>> GetByIdAsync(int id, long usuarioId);
        Task<ApiResponse<ProdutoResponseDto>> AddRangeAsync(List<ProdutoRequestDto?> entities, long usuarioId);
        Task<ApiResponse<ProdutoResponseDto>> UpdateAsync(ProdutoRequestDto? entity, long usuarioId);
        Task<ApiResponse<ProdutoResponseDto>> DeleteAsync(long id, long usuarioId);
    }
}