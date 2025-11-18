using AutoMapper;
using Inventario.Core.DTOs;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.Interfaces.Handlers;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using Inventario.Core.Utils;

namespace Inventario.Core.Handlers
{
    public class EstoqueHandler : IEstoqueHandler
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly IProdutoRepository _produtoRepository;
        
        private readonly IMapper _mapper;

        public EstoqueHandler(IMapper mapper, IUsuarioRepository usuarioRepository, IEstoqueRepository estoqueRepository, IProdutoRepository produtoRepository)
        {
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
            _estoqueRepository = estoqueRepository;
            _produtoRepository = produtoRepository;
        }
        //feto
        public async Task<ApiResponse<List<EstoqueResponseDto>>> GetAllAsync(long usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                    return new ApiResponse<List<EstoqueResponseDto>>(new List<string> { "Usuário não encontrado." });

                var estoques = await _estoqueRepository.GetAllAsync(usuarioId);
                return new ApiResponse<List<EstoqueResponseDto>>(_mapper.Map<List<EstoqueResponseDto>>(estoques));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter estoques por Id do Usuário:", ex);
            }
        }
        //feito
        public async Task<ApiResponse<EstoqueResponseDto>> GetByIdAsync(long id, long usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                    return new ApiResponse<EstoqueResponseDto>(new List<string> { "Usuário não encontrado." });

                var estoque = await _estoqueRepository.GetByIdAsync(id, usuarioId);

                return new ApiResponse<EstoqueResponseDto>(_mapper.Map<EstoqueResponseDto>(estoque));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter estoque por Id:", ex);
            }
        }
        //feito
        public async Task<ApiResponse<EstoqueResponseDto>> AddAsync(EstoqueRequestDto? entity, long usuarioId)
        {
            try
            {
                if (entity is null)
                    return new ApiResponse<EstoqueResponseDto>(new List<string>() { "O Estoque não pode ser nulo." });

                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                    return new ApiResponse<EstoqueResponseDto>(new List<string> { "Usuário não encontrado." });

                var estoque = _mapper.Map<Estoque>(entity);

                var validationResult = await Validate(estoque);
                if (validationResult.IsValid == false)
                    return new ApiResponse<EstoqueResponseDto>(validationResult.Errors);

                estoque.UsuarioId = usuarioId;

                var createdEstoque = await _estoqueRepository.AddAsync(estoque);

                return new ApiResponse<EstoqueResponseDto>(_mapper.Map<EstoqueResponseDto>(createdEstoque));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar estoque:", ex);
            }
        }
            //FEITO
        public async Task<ApiResponse<EstoqueResponseDto>> UpdateAsync(EstoqueRequestDto? entity, long usuarioId)
        {
            try
            {
                if (entity is null)
                    return new ApiResponse<EstoqueResponseDto>(new List<string>() { "O Estoque não pode ser nulo." });

                if (entity.Id is null || entity.Id <= 0)
                    return new ApiResponse<EstoqueResponseDto>(new List<string>() { "O Id do Estoque é obrigatório para atualização." });

                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                    return new ApiResponse<EstoqueResponseDto>(new List<string> { "Usuário não encontrado." });

                var estoqueExistente = await _estoqueRepository.GetByIdAsync((long)entity.Id);
                if (estoqueExistente == null || estoqueExistente.UsuarioId != usuarioId)
                    return new ApiResponse<EstoqueResponseDto>(new List<string>() { "Estoque não encontrado para o usuário." });

                estoqueExistente.Nome = entity.Nome;
                estoqueExistente.UpdatedAt = DateTime.Now;

                var validationResult = await Validate(estoqueExistente);
                if (validationResult.IsValid == false)
                    return new ApiResponse<EstoqueResponseDto>(validationResult.Errors);

                var updatedEstoque = await _estoqueRepository.UpdateAsync(estoqueExistente);

                return new ApiResponse<EstoqueResponseDto>(_mapper.Map<EstoqueResponseDto>(updatedEstoque));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar estoque: {ex.Message}", ex);
            }
        }
        //feito
        public async Task<ApiResponse<EstoqueResponseDto>> DeleteAsync(long id, long usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                    return new ApiResponse<EstoqueResponseDto>(new List<string> { "Usuário não encontrado." });

                var estoqueExistente = await _estoqueRepository.GetByIdAsync(id);
                if (estoqueExistente == null || estoqueExistente.UsuarioId != usuarioId)
                    return new ApiResponse<EstoqueResponseDto>(new List<string>() { "Estoque não encontrado para o usuário." });

                var produtos = await _produtoRepository.GetByEstoqueIdAsync(id, usuarioId);

                if(produtos.Any())
                {
                    foreach(var produto in produtos)
                    {
                        produto.DeletedAt = DateTime.Now;
                        produto.UpdatedAt = DateTime.Now;
                    }
                    await _produtoRepository.UpdateRangeAsync(produtos);
                }
                estoqueExistente.DeletedAt = DateTime.Now;
                var deletedEstoque = await _estoqueRepository.UpdateAsync(estoqueExistente);
                
                var response = _mapper.Map<EstoqueResponseDto>(deletedEstoque);
                response = null;
                return new ApiResponse<EstoqueResponseDto>(response);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar estoque:", ex);
            }
        }
        
        #region private

        private async Task<ValidationResultDto> Validate(Estoque? estoque)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(estoque.Nome))
                return new ValidationResultDto
                {
                    IsValid = false,
                    Errors = new List<string> { "O nome do estorque é obrigatório." }
                };

            return new ValidationResultDto
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }

        #endregion
    }
}