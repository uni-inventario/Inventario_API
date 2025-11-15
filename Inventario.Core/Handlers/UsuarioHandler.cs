using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using AutoMapper;
using Inventario.Core.Utils;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.DTOs;
using Inventario.Core.Validators;
using Microsoft.AspNetCore.Identity;

namespace Inventario.Core.Handlers
{
    public class UsuarioHandler
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

        public async Task<ApiResponse<UsuarioResponseDto>> GetByIdAsync(long id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id);
                if (usuario == null)
                {
                    return new ApiResponse<UsuarioResponseDto>(new List<string> { "Usuário não encontrado." });
                }
                return new ApiResponse<UsuarioResponseDto>(_mapper.Map<UsuarioResponseDto>(usuario));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter usuário por Id:", ex);
            }
        }

        public async Task<ApiResponse<UsuarioResponseDto>> AddAsync(UsuarioRequestDto? entity)
        {
            try
            {
                if (entity is null)
                    return new ApiResponse<UsuarioResponseDto>(new List<string>() { "O Usuário não pode ser nulo." });

                var usuario = _mapper.Map<Usuario>(entity);
                var validationResult = await Validate(usuario);

                if (validationResult.IsValid == false)
                    return new ApiResponse<UsuarioResponseDto>(validationResult.Errors);

                usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);

                var response = await _usuarioRepository.AddAsync(usuario);
                return new ApiResponse<UsuarioResponseDto>(_mapper.Map<UsuarioResponseDto>(response));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir Usuario: {ex.Message}", ex);
            }
        }

        public async Task<ApiResponse<UsuarioResponseDto>> UpdateAsync(UsuarioRequestDto? entity)
        {
            try
            {
                if (entity.Id is null)
                    return new ApiResponse<UsuarioResponseDto>(new List<string>() { "O Id do Usuário é obrigatório para atualização." });

                var usuario = _mapper.Map<Usuario>(entity);
                var validationResult = await Validate(usuario);

                if (validationResult.IsValid == false)
                    return new ApiResponse<UsuarioResponseDto>(validationResult.Errors);

                usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);

                var response = await _usuarioRepository.AddAsync(usuario);
                return new ApiResponse<UsuarioResponseDto>(_mapper.Map<UsuarioResponseDto>(response));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir Usuario: {ex.Message}", ex);
            }
        }

        #region private
        private async Task<ValidationResultDto> Validate(Usuario? usuario, bool isUpdate = false)
        {
            if (isUpdate)
            {
                var usuarioExistente = await _usuarioRepository.GetByIdAsync(usuario.Id);

                if (usuarioExistente == null)
                    return new ValidationResultDto
                    {
                        IsValid = false,
                        Errors = new List<string> { "Usuário não encontrado para atualização." }
                    };
            }

            var validatorUsuario = new UsuarioValidator();

            var validationResultUsuario = validatorUsuario.Validate(usuario);

            var errors = validationResultUsuario.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            if (!isUpdate)
            {
                if (usuario.Senha is null || usuario.Senha.Length < 6)
                {
                    errors.Add("A senha do Usuário é obrigatória e deve ter pelo menos 6 caracteres.");
                }
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