using AutoMapper;
using Inventario.Core.Configurations;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.Handlers;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using Moq;
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;
using Xunit;

namespace Inventario.Test.Handlers
{
    public class EstoqueHandlerTest
    {
        private readonly Core.Interfaces.Handlers.IEstoqueHandler _estoqueHandler;
        private readonly Mock<IEstoqueRepository> _estoqueRepositoryMock = new Mock<IEstoqueRepository>();
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock = new Mock<IProdutoRepository>();
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

        public EstoqueHandlerTest()
        {
            _estoqueHandler = new EstoqueHandler(
                _mapperMock.Object,
                _usuarioRepositoryMock.Object,
                _estoqueRepositoryMock.Object,
                _produtoRepositoryMock.Object
            );
        }

        #region MOCKS AUXILIARES

        private List<Estoque> GetEstoquesMock()
        {
            return new List<Estoque>()
            {
                new Estoque { Id = 1, Nome = "Estoque 1", UsuarioId = 4 },
                new Estoque { Id = 2, Nome = "Estoque 2", UsuarioId = 4 }
            };
        }

        private Estoque GetEstoqueMock()
        {
            return new Estoque
            {
                Id = 1,
                Nome = "Estoque Teste",
                UsuarioId = 4,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        private EstoqueResponseDto GetEstoqueResponseDtoMock(long id = 1, string nome = "Estoque Teste")
        {
            return new EstoqueResponseDto
            {
                Id = id,
                Nome = nome,
            };
        }
        
        private EstoqueRequestDto GetEstoqueRequestDtoMock()
        {
            return new EstoqueRequestDto
            {
                Nome = "Estoque Teste"
            };
        }

        private Usuario GetUsuarioMock()
        {
            return new Usuario
            {
                Id = 4,
                Nome = "Usuário Teste",
                Email = "usuario@gmail.com",
                Senha = "Senha123!"
            };
        }

        private List<Produto> GetProdutosMock(long estoqueId)
        {
            return new List<Produto>
            {
                new Produto { Id = 10, EstoqueId = estoqueId, Nome = "Produto A" },
                new Produto { Id = 11, EstoqueId = estoqueId, Nome = "Produto B"}
            };
        }

        #endregion

        #region TESTES PARA GetAllAsync

        
        /// Testa o cenário de sucesso ao obter todos os estoques de um usuário.
        [Fact]
        public async Task GetAllAsync_Success()
        {
            long usuarioId = 1;
            var estoquesMock = GetEstoquesMock();
            var estoquesResponseMock = estoquesMock.Select(e => new EstoqueResponseDto { Id = e.Id, Nome = e.Nome }).ToList();

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _estoqueRepositoryMock.Setup(x => x.GetAllAsync(usuarioId)).ReturnsAsync(estoquesMock);
            _mapperMock.Setup(m => m.Map<List<EstoqueResponseDto>>(It.IsAny<List<Estoque>>())).Returns(estoquesResponseMock);

            var result = await _estoqueHandler.GetAllAsync(usuarioId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
        }

        
        /// Testa o cenário em que o usuário não é encontrado ao tentar obter estoques.
        [Fact]
        public async Task GetAllAsync_UserNotFound()
        {
            long usuarioId = 1;
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync((Usuario?)null);

            var result = await _estoqueHandler.GetAllAsync(usuarioId);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Usuário não encontrado.", result.Message.First());
        }

        
        /// Testa o cenário de sucesso ao obter uma lista vazia de estoques.
        [Fact]
        public async Task GetAllAsync_EmptyList()
        {
            long usuarioId = 1;
            var emptyList = new List<Estoque>();
            var emptyResponseList = new List<EstoqueResponseDto>();

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _estoqueRepositoryMock.Setup(x => x.GetAllAsync(usuarioId)).ReturnsAsync(emptyList);
            _mapperMock.Setup(m => m.Map<List<EstoqueResponseDto>>(emptyList)).Returns(emptyResponseList);

            var result = await _estoqueHandler.GetAllAsync(usuarioId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        
        /// Testa se o handler lança uma exceção para erros inesperados do repositório.
        [Fact]
        public async Task GetAllAsync_ShouldThrowException()
        {
            long usuarioId = 1;
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ThrowsAsync(new Exception("Erro no repositório"));

            await Assert.ThrowsAsync<Exception>(() => _estoqueHandler.GetAllAsync(usuarioId));
        }

        #endregion

        #region TESTES PARA GetByIdAsync

        
        /// Testa o cenário de sucesso ao obter um estoque por ID.
        [Fact]
        public async Task GetByIdAsync_Success()
        {
            long estoqueId = 1;
            long usuarioId = 4;
            var estoqueMock = GetEstoqueMock();
            var estoqueResponseMock = GetEstoqueResponseDtoMock();

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _estoqueRepositoryMock.Setup(x => x.GetByIdAsync(estoqueId, usuarioId)).ReturnsAsync(estoqueMock);
            _mapperMock.Setup(m => m.Map<EstoqueResponseDto>(estoqueMock)).Returns(estoqueResponseMock);

            var result = await _estoqueHandler.GetByIdAsync(estoqueId, usuarioId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(estoqueId, result.Data.Id);
        }

        
        /// Testa o cenário em que o usuário não é encontrado ao tentar obter o estoque.
        [Fact]
        public async Task GetByIdAsync_UserNotFound()
        {
            long estoqueId = 1;
            long usuarioId = 99;

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync((Usuario?)null);

            var result = await _estoqueHandler.GetByIdAsync(estoqueId, usuarioId);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Usuário não encontrado.", result.Message.First());
            _estoqueRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
        }

        
        /// Testa o cenário em que o estoque não é encontrado.
        [Fact]
        public async Task GetByIdAsync_EstoqueNotFound()
        {
            long estoqueId = 99;
            long usuarioId = 4;

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _estoqueRepositoryMock.Setup(x => x.GetByIdAsync(estoqueId, usuarioId)).ReturnsAsync((Estoque?)null);
            _mapperMock.Setup(m => m.Map<EstoqueResponseDto>((Estoque)null!)).Returns((EstoqueResponseDto)null!);

            var result = await _estoqueHandler.GetByIdAsync(estoqueId, usuarioId);

            Assert.True(result.Success);
            Assert.Null(result.Data);
            Assert.Empty(result.Message ?? new List<string>()); 
        }

        
        /// Testa se o handler lança uma exceção para erros inesperados do repositório.
        [Fact]
        public async Task GetByIdAsync_ShouldThrowException()
        {
            long estoqueId = 1;
            long usuarioId = 4;
            var expectedExceptionMessage = "Erro no repositório";

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ThrowsAsync(new Exception(expectedExceptionMessage));

            var ex = await Assert.ThrowsAsync<Exception>(() => _estoqueHandler.GetByIdAsync(estoqueId, usuarioId));

            Assert.Contains("Erro ao obter estoque por Id:", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        #endregion

        #region TESTES PARA AddAsync

        
        /// Testa o cenário de sucesso na criação de um novo estoque.
        [Fact]
        public async Task AddAsync_Success()
        {
            var request = GetEstoqueRequestDtoMock();
            long usuarioId = 1;
            var estoqueToCreate = GetEstoqueMock();
            var createdEstoque = GetEstoqueMock();
            var responseDto = GetEstoqueResponseDtoMock();

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map<Estoque>(request)).Returns(estoqueToCreate);
            _estoqueRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Estoque>())).ReturnsAsync(createdEstoque);
            _mapperMock.Setup(m => m.Map<EstoqueResponseDto>(createdEstoque)).Returns(responseDto);

            var result = await _estoqueHandler.AddAsync(request, usuarioId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Estoque Teste", result.Data.Nome);
        }

        
        /// Testa o retorno de erro quando o DTO de requisição é nulo.
        [Fact]
        public async Task AddAsync_EntityNulo()
        {
            long usuarioId = 1;

            var result = await _estoqueHandler.AddAsync(null, usuarioId);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("O Estoque não pode ser nulo.", result.Message.First());
        }

        
        /// Testa o retorno de erro quando o usuário é inválido.
        [Fact]
        public async Task AddAsync_UserNotFound()
        {
            var request = GetEstoqueRequestDtoMock();
            long usuarioId = 1;
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync((Usuario?)null);

            var result = await _estoqueHandler.AddAsync(request, usuarioId);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Usuário não encontrado.", result.Message.First());
        }

        
        /// Testa o retorno de erro quando a validação de negócio falha (ex: Nome vazio).
        [Fact]
        public async Task AddAsync_ValidationFails()
        {
            var request = GetEstoqueRequestDtoMock();
            long usuarioId = 1;
            request.Nome = ""; 
            var invalidEstoque = new Estoque { Nome = "" };

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map<Estoque>(request)).Returns(invalidEstoque);
            
            var result = await _estoqueHandler.AddAsync(request, usuarioId);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.True(result.Message.Count > 0);
            Assert.Contains("O nome do estorque é obrigatório.", result.Message);
        }

        
        /// Testa se o handler lança uma exceção para erros inesperados do repositório.
        [Fact]
        public async Task AddAsync_ShouldThrowException()
        {
            var request = GetEstoqueRequestDtoMock();
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ThrowsAsync(new Exception("Erro inesperado"));

            await Assert.ThrowsAsync<Exception>(() => _estoqueHandler.AddAsync(request, 1));
        }

        #endregion

        #region TESTES PARA UpdateAsync

        
        /// Testa o cenário de sucesso na atualização de um estoque existente.
        [Fact]
        public async Task UpdateAsync_Success()
        {
            long usuarioId = 4;
            var request = new EstoqueRequestDto { Id = 1, Nome = "Novo Nome do Estoque" };
            var estoqueExistente = GetEstoqueMock();
            var estoqueAtualizado = new Estoque { Id = 1, Nome = "Novo Nome do Estoque", UsuarioId = usuarioId, UpdatedAt = DateTime.Now };
            var estoqueResponse = GetEstoqueResponseDtoMock(1, "Novo Nome do Estoque");

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _estoqueRepositoryMock.Setup(x => x.GetByIdAsync((long)request.Id)).ReturnsAsync(estoqueExistente);
            _estoqueRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Estoque>())).ReturnsAsync(estoqueAtualizado);
            _mapperMock.Setup(m => m.Map<EstoqueResponseDto>(It.IsAny<Estoque>())).Returns(estoqueResponse);
            
            var result = await _estoqueHandler.UpdateAsync(request, usuarioId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(request.Nome, result.Data.Nome);
            _estoqueRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Estoque>(e => e.Nome == request.Nome)), Times.Once);
        }

        
        /// Testa o retorno de erro quando o DTO de requisição para atualização é nulo.
        [Fact]
        public async Task UpdateAsync_EntityIsNull()
        {
            var result = await _estoqueHandler.UpdateAsync(null, 1);

            Assert.False(result.Success);
            Assert.Contains("O Estoque não pode ser nulo.", result.Message.First());
            _estoqueRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Estoque>()), Times.Never);
        }

        
        /// Testa o retorno de erro quando o Id do Estoque para atualização é inválido (null, 0, ou negativo).
        [Fact]
        public async Task UpdateAsync_IdIsMissingOrInvalid()
        {
            var request = new EstoqueRequestDto { Id = null, Nome = "Nome de teste" };

            var result = await _estoqueHandler.UpdateAsync(request, 1);

            Assert.False(result.Success);
            Assert.Contains("O Id do Estoque é obrigatório para atualização.", result.Message.First());
        }

        
        /// Testa o retorno de erro quando o usuário da requisição não é encontrado.
        [Fact]
        public async Task UpdateAsync_UserNotFound()
        {
            long usuarioId = 99;
            var request = new EstoqueRequestDto { Id = 1, Nome = "Novo Nome" };
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync((Usuario?)null);

            var result = await _estoqueHandler.UpdateAsync(request, usuarioId);

            Assert.False(result.Success);
            Assert.Contains("Usuário não encontrado.", result.Message.First());
            _estoqueRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<long>()), Times.Never);
        }

        
        /// Testa o retorno de erro quando o estoque não existe ou não pertence ao usuário.
        [Fact]
        public async Task UpdateAsync_EstoqueNotFoundOrNotBelongingToUser()
        {
            long usuarioId = 4;
            var request = new EstoqueRequestDto { Id = 99, Nome = "Novo Nome" };

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _estoqueRepositoryMock.Setup(x => x.GetByIdAsync((long)request.Id)).ReturnsAsync((Estoque?)null);

            var result = await _estoqueHandler.UpdateAsync(request, usuarioId);

            Assert.False(result.Success);
            Assert.Contains("Estoque não encontrado para o usuário.", result.Message.First());
            _estoqueRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Estoque>()), Times.Never);
        }

        
        /// Testa o retorno de erro quando a validação do modelo atualizado falha.
        [Fact]
        public async Task UpdateAsync_ValidationFails()
        {
            long usuarioId = 4;
            var request = new EstoqueRequestDto { Id = 1, Nome = "" };
            var estoqueExistente = GetEstoqueMock(); 
            estoqueExistente.Nome = ""; // Simula o nome sendo atualizado para vazio (inválido)
            
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _estoqueRepositoryMock.Setup(x => x.GetByIdAsync((long)request.Id)).ReturnsAsync(estoqueExistente);

            var result = await _estoqueHandler.UpdateAsync(request, usuarioId);

            Assert.False(result.Success);
            Assert.True(result.Message.Count > 0);
            _estoqueRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Estoque>()), Times.Never);
        }

        
        /// Testa se o handler lança uma exceção para erros inesperados durante a atualização.
        [Fact]
        public async Task UpdateAsync_ShouldThrowException()
        {
            long usuarioId = 4;
            var request = new EstoqueRequestDto { Id = 1, Nome = "Nome Válido" };
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ThrowsAsync(new Exception("Erro no repositório de usuário"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _estoqueHandler.UpdateAsync(request, usuarioId));

            Assert.Contains("Erro ao atualizar estoque:", ex.Message);
            Assert.NotNull(ex.InnerException);
        }
        #endregion

        #region TESTES PARA DeleteAsync

        
        /// Testa o cenário de sucesso na exclusão suave (soft delete) de um estoque sem produtos relacionados.
        [Fact]
        public async Task DeleteAsync_Success_NoProducts()
        {
            long estoqueId = 1;
            long usuarioId = 4;
            var estoqueExistente = GetEstoqueMock();
            
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _estoqueRepositoryMock.Setup(x => x.GetByIdAsync(estoqueId)).ReturnsAsync(estoqueExistente);
            _produtoRepositoryMock.Setup(x => x.GetByEstoqueIdAsync(estoqueId, usuarioId)).ReturnsAsync(new List<Produto>());
            _estoqueRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Estoque>())).ReturnsAsync(estoqueExistente);
            _mapperMock.Setup(m => m.Map<EstoqueResponseDto>(It.IsAny<Estoque>())).Returns(GetEstoqueResponseDtoMock());

            var result = await _estoqueHandler.DeleteAsync(estoqueId, usuarioId);

            Assert.True(result.Success);
            Assert.Null(result.Data); 
            Assert.Empty(result.Message ?? new List<string>()); 

            _estoqueRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Estoque>(e => e.DeletedAt.HasValue)), Times.Once); 
            _produtoRepositoryMock.Verify(x => x.UpdateRangeAsync(It.IsAny<List<Produto>>()), Times.Never); 
        }

        
        /// Testa o cenário de sucesso na exclusão suave, garantindo que os produtos relacionados também são deletados.
        [Fact]
        public async Task DeleteAsync_Success_WithProducts()
        {
            long estoqueId = 1;
            long usuarioId = 4;
            var estoqueExistente = GetEstoqueMock();
            var produtosMock = GetProdutosMock(estoqueId);

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _estoqueRepositoryMock.Setup(x => x.GetByIdAsync(estoqueId)).ReturnsAsync(estoqueExistente);
            _produtoRepositoryMock.Setup(x => x.GetByEstoqueIdAsync(estoqueId, usuarioId)).ReturnsAsync(produtosMock);
            _estoqueRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Estoque>())).ReturnsAsync(estoqueExistente);
            _mapperMock.Setup(m => m.Map<EstoqueResponseDto>(It.IsAny<Estoque>())).Returns(GetEstoqueResponseDtoMock());

            var result = await _estoqueHandler.DeleteAsync(estoqueId, usuarioId);

            Assert.True(result.Success);
            Assert.Null(result.Data); 
            Assert.Empty(result.Message ?? new List<string>());

            _produtoRepositoryMock.Verify(x => x.UpdateRangeAsync(
                It.Is<List<Produto>>(p => p.All(prod => prod.DeletedAt.HasValue))), Times.Once); 
            _estoqueRepositoryMock.Verify(x => x.UpdateAsync(
                It.Is<Estoque>(e => e.DeletedAt.HasValue)), Times.Once); 
        }

        
        /// Testa o retorno de erro quando o usuário da requisição não é encontrado.
        [Fact]
        public async Task DeleteAsync_UserNotFound()
        {
            long usuarioId = 99;
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync((Usuario?)null);

            var result = await _estoqueHandler.DeleteAsync(1, usuarioId);

            Assert.False(result.Success);
            Assert.Contains("Usuário não encontrado.", result.Message.First());
            _estoqueRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<long>()), Times.Never);
        }

        
        /// Testa o retorno de erro quando o estoque não é encontrado pelo ID ou não pertence ao usuário.
        [Fact]
        public async Task DeleteAsync_EstoqueNotFound()
        {
            long estoqueId = 99;
            long usuarioId = 4;

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _estoqueRepositoryMock.Setup(x => x.GetByIdAsync(estoqueId)).ReturnsAsync((Estoque?)null);

            var result = await _estoqueHandler.DeleteAsync(estoqueId, usuarioId);

            Assert.False(result.Success);
            Assert.Contains("Estoque não encontrado para o usuário.", result.Message.First());
            _estoqueRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Estoque>()), Times.Never);
        }

        
        /// Testa o retorno de erro quando o estoque pertence a outro usuário (Unauthorized).
        [Fact]
        public async Task DeleteAsync_EstoqueNotBelongingToUser()
        {
            long estoqueId = 1;
            long usuarioId = 99;
            var estoqueDoOutroUsuario = GetEstoqueMock(); 
            estoqueDoOutroUsuario.UsuarioId = 4; // Pertence ao usuário 4

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(new Usuario { Id = usuarioId });
            _estoqueRepositoryMock.Setup(x => x.GetByIdAsync(estoqueId)).ReturnsAsync(estoqueDoOutroUsuario); 

            var result = await _estoqueHandler.DeleteAsync(estoqueId, usuarioId);

            Assert.False(result.Success);
            Assert.Contains("Estoque não encontrado para o usuário.", result.Message.First());
            _estoqueRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Estoque>()), Times.Never);
        }

        
        /// Testa se o handler lança uma exceção para erros inesperados durante a exclusão.
        [Fact]
        public async Task DeleteAsync_ShouldThrowException()
        {
            long estoqueId = 1;
            long usuarioId = 4;
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ThrowsAsync(new Exception("Erro de conexão"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _estoqueHandler.DeleteAsync(estoqueId, usuarioId));

            Assert.Contains("Erro ao deletar estoque:", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        #endregion
    }
}