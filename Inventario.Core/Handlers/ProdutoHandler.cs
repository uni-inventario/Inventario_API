using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inventario.Core.DTOs;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.Interfaces.Handlers;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using Inventario.Core.Utils;
using Inventario.Core.Validators;

namespace Inventario.Core.Handlers
{
    public class ProdutoHandler : IProdutoHandler
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public ProdutoHandler(IMapper mapper, IProdutoRepository produtoRepository, IEstoqueRepository estoqueRepository, IUsuarioRepository usuarioRepository)
        {
            _mapper = mapper;
            _produtoRepository = produtoRepository;
            _estoqueRepository = estoqueRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<ApiResponse<ProdutoResponseDto>> GetByIdAsync(int id, long usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                    return new ApiResponse<ProdutoResponseDto>(new List<string> { "Usuário não encontrado." });

                var produto = await _produtoRepository.GetByIdAsync(id, usuarioId);

                return new ApiResponse<ProdutoResponseDto>(_mapper.Map<ProdutoResponseDto>(produto));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter produto por Id:", ex);
            }
        }

        public async Task<ApiResponse<ProdutoResponseDto>> AddRangeAsync(List<ProdutoRequestDto?> entities, long usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                    return new ApiResponse<ProdutoResponseDto>(new List<string> { "Usuário não encontrado." });

                var produtos = _mapper.Map<List<Produto>>(entities);

                foreach (var produto in produtos)
                {
                    var validationResult = await Validate(produto);

                    if (validationResult.IsValid == false)
                        return new ApiResponse<ProdutoResponseDto>(validationResult.Errors);

                    var estoque = await _estoqueRepository.GetByIdAsync(produto.EstoqueId, usuarioId);
                    if (estoque == null && estoque?.UsuarioId != usuarioId)
                        return new ApiResponse<ProdutoResponseDto>(new List<string> { $"Estoque não encontrado para o Produto {produto.Nome}." });
                }

                await _produtoRepository.AddRangeAsync(produtos);
                
                var response = new ProdutoResponseDto();
                response = null;
                return new ApiResponse<ProdutoResponseDto>(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir Produtos: {ex.Message}", ex);
            }
        }

        public async Task<ApiResponse<ProdutoResponseDto>> UpdateAsync(ProdutoRequestDto? entity, long usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                    return new ApiResponse<ProdutoResponseDto>(new List<string> { "Usuário não encontrado." });

                if (entity is null)
                    return new ApiResponse<ProdutoResponseDto>(new List<string>() { "O Produto não pode ser nulo." });

                if (entity.Id is null || entity.Id <= 0)
                    return new ApiResponse<ProdutoResponseDto>(new List<string>() { "O Id do Produto é obrigatório para atualização." });

                var produto = _mapper.Map<Produto>(entity);
                var validationResult = await Validate(produto);

                if (validationResult.IsValid == false)
                    return new ApiResponse<ProdutoResponseDto>(validationResult.Errors);

                var produtoExistente = await _produtoRepository.GetByIdAsync((long)entity.Id);

                if (produtoExistente == null)
                    return new ApiResponse<ProdutoResponseDto>(new List<string>() { "Produto não encontrado." });

                var estoque = await _estoqueRepository.GetByIdAsync(produto.EstoqueId, usuarioId);
                if (estoque == null)
                    return new ApiResponse<ProdutoResponseDto>(new List<string> { "Estoque não encontrado para o Produto." });

                produtoExistente.Nome = produto.Nome;
                produtoExistente.Descricao = produto.Descricao;
                produtoExistente.Quantidade = produto.Quantidade;
                produtoExistente.UpdatedAt = DateTime.Now;
                produtoExistente.Preco = produto.Preco;

                var response = await _produtoRepository.UpdateAsync(produtoExistente);
                return new ApiResponse<ProdutoResponseDto>(_mapper.Map<ProdutoResponseDto>(response));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar Produto: {ex.Message}", ex);
            }
        }

        public async Task<ApiResponse<ProdutoResponseDto>> DeleteAsync(long id, long usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                    return new ApiResponse<ProdutoResponseDto>(new List<string> { "Usuário não encontrado." });

                var produtoExistente = await _produtoRepository.GetByIdAsync(id, usuarioId);
                if (produtoExistente == null)
                    return new ApiResponse<ProdutoResponseDto>(new List<string>() { "Produto não encontrado para o usuário." });

                produtoExistente.DeletedAt = DateTime.Now;
                produtoExistente.UpdatedAt = DateTime.Now;

                var deletedProduto = await _produtoRepository.UpdateAsync(produtoExistente);

                var response = _mapper.Map<ProdutoResponseDto>(deletedProduto);
                response = null;
                return new ApiResponse<ProdutoResponseDto>(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar Produto: {ex.Message}", ex);
            }
        }
        
        #region private
        private async Task<ValidationResultDto> Validate(Produto? produto)
        {
            if (produto is null)
                throw new Exception("O produto não pode ser nulo.");

            var validatorProduto = new ProdutoValidator();

            var validationResultProduto = validatorProduto.Validate(produto);

            var errors = validationResultProduto.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            return new ValidationResultDto
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }

        #endregion

    }
}