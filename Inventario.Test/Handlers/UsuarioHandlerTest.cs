using AutoMapper;
using Inventario.Core.Configurations;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.Handlers;
using Inventario.Core.Interfaces.Handlers; 
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using Moq;
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;
using Xunit;
using FluentValidation.Results;


namespace Inventario.Test.Handlers
{
    public class UsuarioHandlerTest
    {
        private readonly Core.Interfaces.Handlers.IUsuarioHandler _usuarioHandler;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

        public UsuarioHandlerTest()
        {
            _usuarioHandler = new UsuarioHandler(_mapperMock.Object, _usuarioRepositoryMock.Object);
        }

        #region MOCKS AUXILIARES
        
            private UsuarioRequestDto GetFakeUsuarioRequestDto(
                string nome = "Novo Usuário", 
                string email = "novo@exemplo.com", 
                string senha = "SenhaForte123") =>
                new UsuarioRequestDto { Nome = nome, Email = email, Senha = senha };
            
            private Usuario GetFakeUsuarioModel(long id, string nome = "Teste", string email = "teste@exemplo.com") =>
                new Usuario { Id = id, Nome = nome, Email = email };

            private UsuarioResponseDto GetFakeUsuarioResponseDto(
                long id, 
                string nome = "Teste", 
                string email = "teste@exemplo.com") =>
                new UsuarioResponseDto { Id = id, Nome = nome, Email = email };

        #endregion

        #region testes para GetByIdAsync
        
        // 1. Sucesso: O usuário é encontrado e mapeado corretamente.
        [Fact]
        public async Task GetByIdAsync_DeveRetornarUsuario_QuandoEncontrado()
        {
            long usuarioId = 1;
            var usuarioModel = GetFakeUsuarioModel(usuarioId);
            var usuarioDto = GetFakeUsuarioResponseDto(usuarioId);

            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(usuarioId))
                .ReturnsAsync(usuarioModel);

            _mapperMock.Setup(m => m.Map<UsuarioResponseDto>(usuarioModel))
                .Returns(usuarioDto);

            var result = await _usuarioHandler.GetByIdAsync(usuarioId);

            Assert.True(result.Success);
            Assert.Equal(usuarioDto.Id, result.Data.Id);
            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);
            _mapperMock.Verify(m => m.Map<UsuarioResponseDto>(usuarioModel), Times.Once);
        }

        // 2. Não Encontrado: O repositório retorna null.
        [Fact]
        public async Task GetByIdAsync_DeveRetornarErro_QuandoUsuarioNaoEncontrado()
        {
            long usuarioId = 99;

            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(usuarioId))
                .ReturnsAsync((Usuario)null);

            var result = await _usuarioHandler.GetByIdAsync(usuarioId);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Usuário não encontrado.", result.Message);
            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);

            _mapperMock.Verify(m => m.Map<UsuarioResponseDto>(It.IsAny<Usuario>()), Times.Never);
        }

        // 3. Id Nulo: Testa o comportamento quando o 'id' é null.
        [Fact]
        public async Task GetByIdAsync_DeveLancarExcecaoOuErro_QuandoIdENulo()
        {
            long? usuarioId = null;

            await Assert.ThrowsAsync<System.Exception>(() => _usuarioHandler.GetByIdAsync(usuarioId));

            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
            _mapperMock.Verify(m => m.Map<UsuarioResponseDto>(It.IsAny<Usuario>()), Times.Never);
        }
        #endregion
        #region testes para AddAsync
        [Fact]
        public async Task AddAsync_DeveCriarUsuarioComSucesso_QuandoDadosValidos()
        {
            long novoId = 5;
            var requestDto = GetFakeUsuarioRequestDto();
            var usuarioModelPreHash = new Usuario { Nome = requestDto.Nome, Email = requestDto.Email, Senha = requestDto.Senha };
            var usuarioModelPosHash = GetFakeUsuarioModel(novoId);
            var responseDto = GetFakeUsuarioResponseDto(novoId);
            
            _mapperMock.Setup(m => m.Map<Usuario>(requestDto))
                .Returns(usuarioModelPreHash);

            _usuarioRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Usuario>()))
                .ReturnsAsync(usuarioModelPosHash);

            _mapperMock.Setup(m => m.Map<UsuarioResponseDto>(usuarioModelPosHash))
                .Returns(responseDto);

            var result = await _usuarioHandler.AddAsync(requestDto);

            Assert.True(result.Success);
            Assert.Equal(novoId, result.Data.Id);
            Assert.Equal(responseDto.Nome, result.Data.Nome);

            _usuarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Once);

            _mapperMock.Verify(m => m.Map<Usuario>(requestDto), Times.Once);
            _mapperMock.Verify(m => m.Map<UsuarioResponseDto>(usuarioModelPosHash), Times.Once);
            
            _usuarioRepositoryMock.Verify(r => r.AddAsync(It.Is<Usuario>(u => u.Senha != requestDto.Senha)), Times.Once);
        }


        [Fact]
        public async Task AddAsync_DeveRetornarErro_QuandoRequestDtoENulo()
        {
            UsuarioRequestDto? entity = null;

            var result = await _usuarioHandler.AddAsync(entity);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("O Usuário não pode ser nulo.", result.Message);

            _mapperMock.Verify(m => m.Map<Usuario>(It.IsAny<UsuarioRequestDto>()), Times.Never);
            _usuarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
        }


        #endregion

        #region Testes para UpdateAsync

        // 1. Sucesso: O usuário existe, é atualizado e o repositório é chamado.
        [Fact]
        public async Task UpdateAsync_DeveAtualizarUsuarioComSucesso_QuandoEncontradoEValido()
        {
            long usuarioId = 1;
            var requestDto = GetFakeUsuarioRequestDto("Nome Atualizado", "email.novo@update.com");
            var usuarioExistente = GetFakeUsuarioModel(usuarioId);
            
            var usuarioAtualizado = usuarioExistente; 
            usuarioAtualizado.Nome = requestDto.Nome;
            usuarioAtualizado.Email = requestDto.Email; 
            
            var responseDto = GetFakeUsuarioResponseDto(usuarioId, usuarioAtualizado.Nome, usuarioAtualizado.Email);

            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(usuarioId))
                .ReturnsAsync(usuarioExistente);


            _usuarioRepositoryMock.Setup(r => r.UpdateAsync(It.Is<Usuario>(u => u.Id == usuarioId)))
                .ReturnsAsync(usuarioAtualizado);

            _mapperMock.Setup(m => m.Map<UsuarioResponseDto>(usuarioAtualizado))
                .Returns(responseDto);


            var result = await _usuarioHandler.UpdateAsync(requestDto, usuarioId);


            Assert.True(result.Success);
            Assert.Equal(usuarioId, result.Data.Id);
            Assert.Equal(requestDto.Nome, result.Data.Nome);
            
            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.UpdateAsync(usuarioAtualizado), Times.Once);

            _mapperMock.Verify(m => m.Map<UsuarioResponseDto>(usuarioAtualizado), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarErro_QuandoRequestDtoENulo()
        {

            UsuarioRequestDto? entity = null;
            long usuarioId = 1;


            var result = await _usuarioHandler.UpdateAsync(entity, usuarioId);


            Assert.False(result.Success);
            Assert.Null(result.Data);

            Assert.Contains("O Id do Usuário é obrigatório para atualização.", result.Message); 

            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
            _usuarioRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }

        // 3. Erro: Usuário não encontrado para o ID fornecido
        [Fact]
        public async Task UpdateAsync_DeveRetornarErro_QuandoUsuarioNaoEncontrado()
        {
            long usuarioId = 99;
            var requestDto = GetFakeUsuarioRequestDto();

            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(usuarioId))
                .ReturnsAsync((Usuario)null);

            var result = await _usuarioHandler.UpdateAsync(requestDto, usuarioId);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Usuário não encontrado para atualização.", result.Message); 

            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }

        // 4. Erro: Tratamento de exceção
        [Fact]
        public async Task UpdateAsync_DeveLancarExcecao_EmCasoDeErroNoRepositorio()
        {
            long usuarioId = 1;
            var requestDto = GetFakeUsuarioRequestDto();
            var exceptionMessage = "Simulação de falha de conexão com o banco de dados.";
            
            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(usuarioId))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            var exception = await Assert.ThrowsAsync<System.Exception>(() => _usuarioHandler.UpdateAsync(requestDto, usuarioId));

            Assert.Contains("Erro ao atualizar usuário", exception.Message);
            Assert.Equal(exceptionMessage, exception.InnerException.Message);
            
            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }

        
        #endregion
        #region Testes para DeleteAsync

        // 1. Sucesso: Usuário encontrado e exclusão lógica aplicada.
        [Fact]
        public async Task DeleteAsync_DeveExecutarExclusaoLogica_QuandoUsuarioEncontrado()
        {
            long usuarioId = 1;
            var usuarioExistente = GetFakeUsuarioModel(usuarioId);
            
            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(usuarioId))
                .ReturnsAsync(usuarioExistente);

            _usuarioRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Usuario>()))
                .ReturnsAsync(usuarioExistente); 

            _mapperMock.Setup(m => m.Map<UsuarioResponseDto>(It.IsAny<Usuario>()))
                .Returns(new UsuarioResponseDto { Id = usuarioId });


            var result = await _usuarioHandler.DeleteAsync(usuarioId);

            Assert.True(result.Success);
            
            Assert.Null(result.Data);

            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);

            _usuarioRepositoryMock.Verify(r => r.UpdateAsync(
                It.Is<Usuario>(u => u.Id == usuarioId && u.DeletedAt != null) 
            ), Times.Once);
        }

        // 2. Erro: Usuário não encontrado para o ID fornecido.
        [Fact]
        public async Task DeleteAsync_DeveRetornarErro_QuandoUsuarioNaoEncontrado()
        {
            long usuarioId = 99;

            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(usuarioId))
                .ReturnsAsync((Usuario)null);

            var result = await _usuarioHandler.DeleteAsync(usuarioId);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Usuário não encontrado para exclusão.", result.Message); 

            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }

        // 3. Erro: Tratamento de exceção em caso de falha no repositório.
        [Fact]
        public async Task DeleteAsync_DeveLancarExcecao_EmCasoDeErroNoRepositorio()
        {
            long usuarioId = 1;
            var exceptionMessage = "Simulação de falha ao buscar no banco de dados.";
            
            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(usuarioId))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            var exception = await Assert.ThrowsAsync<System.Exception>(() => _usuarioHandler.DeleteAsync(usuarioId));

            Assert.Contains("Erro ao excluir usuário", exception.Message);
            Assert.Equal(exceptionMessage, exception.InnerException.Message);
            
            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }

        #endregion
    }
}