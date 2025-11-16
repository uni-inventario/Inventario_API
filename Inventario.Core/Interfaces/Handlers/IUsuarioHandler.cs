using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.Utils;

namespace Inventario.Core.Interfaces.Handlers
{
    public interface IUsuarioHandler
    {
        Task<ApiResponse<UsuarioResponseDto>> GetByIdAsync(long? id);
        Task<ApiResponse<UsuarioResponseDto>> AddAsync(UsuarioRequestDto? entity);
       Task<ApiResponse<UsuarioResponseDto>> UpdateAsync(UsuarioRequestDto? entity, long userId);


    }
}