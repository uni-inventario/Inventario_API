using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.Models;

namespace Inventario.Core.Configurations
{
 public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<Usuario, UsuarioResponseDto>();
            CreateMap<Estoque, EstoqueResponseDto>();
            CreateMap<Produto, ProdutoResponseDto>();
            CreateMap<ProdutoRequestDto, Produto>();
            CreateMap<EstoqueRequestDto, Estoque>();
            CreateMap<UsuarioRequestDto, Usuario>();
        }
    }
}