using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using AutoMapper;
using Inventario.Core.Utils;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.DTOs;
using Inventario.Core.Validators;
using Microsoft.AspNetCore.Identity;
using Inventario.Core.Interfaces.Handlers;

namespace Inventario.Core.Handlers
{
    public class UsuarioHandler : IUsuarioHandler
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public UsuarioHandler(IMapper mapper, IUsuarioRepository usuarioRepository)
        {
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        #region GET
        public async Task<ApiResponse<UsuarioResponseDto>> GetByIdAsync(long? id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id.Value);

                if (usuario == null)
                    return new ApiResponse<UsuarioResponseDto>(new List<string> { "Usuário não encontrado." });

                return new ApiResponse<UsuarioResponseDto>(_mapper.Map<UsuarioResponseDto>(usuario));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter usuário por Id.", ex);
            }
        }
        #endregion

        #region CREATE
        public async Task<ApiResponse<UsuarioResponseDto>> AddAsync(UsuarioRequestDto? entity)
        {
            try
            {
                if (entity is null)
                    return new ApiResponse<UsuarioResponseDto>(new List<string>() { "O Usuário não pode ser nulo." });

                var usuario = _mapper.Map<Usuario>(entity);
                var validationResult = await Validate(usuario);
                if (!validationResult.IsValid)
                    return new ApiResponse<UsuarioResponseDto>(validationResult.Errors);

                usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);

                var response = await _usuarioRepository.AddAsync(usuario);
                return new ApiResponse<UsuarioResponseDto>(_mapper.Map<UsuarioResponseDto>(response));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir usuário: {ex.Message}", ex);
            }
        }
        #endregion

        #region UPDATE
        public async Task<ApiResponse<UsuarioResponseDto>> UpdateAsync(UsuarioRequestDto? entity, long userId)
        {
            try
            {
                if (entity is null)
                    return new ApiResponse<UsuarioResponseDto>(new List<string> { "O Id do Usuário é obrigatório para atualização." });

                var usuarioExistente = await _usuarioRepository.GetByIdAsync(userId);

                if (usuarioExistente is null)
                    return new ApiResponse<UsuarioResponseDto>(new List<string> { "Usuário não encontrado para atualização." });

                usuarioExistente.Nome = entity.Nome;
                usuarioExistente.Email = entity.Email;
                usuarioExistente.UpdatedAt = DateTime.Now;

                var validationResult = await Validate(usuarioExistente, true);
                if (!validationResult.IsValid)
                    return new ApiResponse<UsuarioResponseDto>(validationResult.Errors);

                var response = await _usuarioRepository.UpdateAsync(usuarioExistente);
                return new ApiResponse<UsuarioResponseDto>(_mapper.Map<UsuarioResponseDto>(response));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar usuário: {ex.Message}", ex);
            }
        }
        #endregion

        #region VALIDATION
        private async Task<ValidationResultDto> Validate(Usuario usuario, bool isUpdate = false)
        {
            var validator = new UsuarioValidator();
            var validation = validator.Validate(usuario);

            var errors = validation.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            if (!isUpdate)
            {
                if (string.IsNullOrWhiteSpace(usuario.Senha) || usuario.Senha.Length < 6)
                    errors.Add("A senha é obrigatória e deve conter pelo menos 6 caracteres.");
            }

            return new ValidationResultDto
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }
        #endregion
    }
}