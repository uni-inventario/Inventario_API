using AutoMapper;
using Inventario.Core.Configurations;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.DTOs.Responses;
using Inventario.Core.Handlers;
using Inventario.Core.Interfaces.Handlers;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Inventario.Test.Handlers
{
    public class ProdutoHandlerTest
    {
        private readonly IProdutoHandler _produtoHandler;
        private readonly Mock<IEstoqueRepository> _estoqueRepositoryMock = new Mock<IEstoqueRepository>();
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock = new Mock<IProdutoRepository>();
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

        public ProdutoHandlerTest()
        {
            _produtoHandler = new ProdutoHandler(
                _mapperMock.Object,
                _produtoRepositoryMock.Object,
                _estoqueRepositoryMock.Object,
                _usuarioRepositoryMock.Object
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

        private Produto GetProdutoMock()
        {
            return new Produto
            {
                Id = 1,
                EstoqueId = 1,
                Nome = "Produto A",
                Descricao = "Descrição do Produto",
                Quantidade = 10,
                Preco = 20m,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        private ProdutoResponseDto GetProdutoResponseDtoMock()
        {
            return new ProdutoResponseDto
            {
                Id = 1,
                Nome = "Produto A",
                Descricao = "Descrição do Produto",
                Quantidade = 10,
                Preco = 20m,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        private ProdutoRequestDto GetProdutoRequestDtoMock()
        {
            return new ProdutoRequestDto
            {
                Id = 1,
                Nome = "Produto A",
                Descricao = "Descrição do Produto",
                Quantidade = 10,
                Preco = 20m
            };
        }

        #endregion

        #region TESTES PARA GetByIdAsync


        /// Testa o cenário de sucesso ao obter um produto por ID.
        [Fact]
        public async Task GetByIdAsync_Success()
        {
            // Preparar
            int produtoId = 1;
            long usuarioId = 4;
            var produtoMock = GetProdutoMock();

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _produtoRepositoryMock.Setup(x => x.GetByIdAsync(produtoId, usuarioId)).ReturnsAsync(produtoMock);
            _mapperMock.Setup(m => m.Map<ProdutoResponseDto>(produtoMock)).Returns(GetProdutoResponseDtoMock());

            // Agir
            var result = await _produtoHandler.GetByIdAsync(produtoId, usuarioId);

            // Verificar
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(produtoId, result.Data.Id);
        }


        /// Testa o cenário em que o usuário não é encontrado ao tentar obter o produto.
        [Fact]
        public async Task GetByIdAsync_UserNotFound()
        {
            // Preparar
            int produtoId = 1;
            long usuarioId = 4;
            var produtoMock = GetProdutoMock();

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync((Usuario?)null);
            _produtoRepositoryMock.Setup(x => x.GetByIdAsync(produtoId, usuarioId)).ReturnsAsync(produtoMock);
            _mapperMock.Setup(m => m.Map<ProdutoResponseDto>(produtoMock)).Returns(GetProdutoResponseDtoMock());

            // Agir
            var result = await _produtoHandler.GetByIdAsync(produtoId, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Usuário não encontrado.", result.Message.First());
            _estoqueRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
        }


        /// Testa se o handler lança uma exceção para erros inesperados do repositório.
        [Fact]
        public async Task GetByIdAsync_ShouldThrowException()
        {
            // Preparar
            int produtoId = 1;
            long usuarioId = 4;
            var expectedExceptionMessage = "Erro no repositório";

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ThrowsAsync(new Exception(expectedExceptionMessage));

            // Agir & Verificar
            var ex = await Assert.ThrowsAsync<Exception>(() => _produtoHandler.GetByIdAsync(produtoId, usuarioId));

            Assert.Contains("Erro ao obter produto por Id:", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        #endregion

        #region TESTES PARA AddRangeAsync

        /// Testa o cenário de sucesso na criação de um novo estoque.
        [Fact]
        public async Task AddAsync_Success()
        {
            // Preparar
            var request = new List<ProdutoRequestDto?> { GetProdutoRequestDtoMock() };
            long usuarioId = 1;
            var produto = new List<Produto> { GetProdutoMock() };
            var estoqueResponse = GetEstoqueMock();
            var responseDto = GetProdutoResponseDtoMock();

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map< List<Produto>>(request)).Returns(produto);
            _estoqueRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(estoqueResponse);
            _mapperMock.Setup(m => m.Map<ProdutoResponseDto>(produto)).Returns(responseDto);
            _produtoRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<Produto>>()));

            // Agir
            var result = await _produtoHandler.AddRangeAsync(request, usuarioId);

            // Verificar
            Assert.True(result.Success);
            Assert.Null(result.Data);
        }


        /// Testa o retorno de erro quando o usuário é inválido.
        [Fact]
        public async Task AddAsync_UserNotFound()
        {
            // Preparar
            var request = new List<ProdutoRequestDto?> {GetProdutoRequestDtoMock()};
            long usuarioId = 1;
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync((Usuario?)null);

            // Agir
            var result = await _produtoHandler.AddRangeAsync(request, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Usuário não encontrado.", result.Message.First());
        }


        /// Testa o retorno de erro quando a validação de negócio falha (ex: Nome vazio).
        [Fact]
        public async Task AddAsync_ValidationFailsNome()
        {
            // Preparar
            var request = new List<ProdutoRequestDto?> { GetProdutoRequestDtoMock() };
            long usuarioId = 1;
            var produtoRequest = GetProdutoMock();
            produtoRequest.Nome = null;
            var produto = new List<Produto> { produtoRequest };

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map<List<Produto>>(request)).Returns(produto);

            // Agir
            var result = await _produtoHandler.AddRangeAsync(request, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.True(result.Message.Count > 0);
            Assert.Contains("O nome do produto é obrigatório.", result.Message.First());
        }

        /// Testa o retorno de erro quando a validação de negócio falha (ex: Descrição vazia).
        [Fact]
        public async Task AddAsync_ValidationFailsDescricao()
        {
            // Preparar
            var request = new List<ProdutoRequestDto?> { GetProdutoRequestDtoMock() };
            long usuarioId = 1;
            var produtoRequest = GetProdutoMock();
            produtoRequest.Descricao = null;
            var produto = new List<Produto> { produtoRequest };

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map<List<Produto>>(request)).Returns(produto);

            // Agir
            var result = await _produtoHandler.AddRangeAsync(request, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.True(result.Message.Count > 0);
            Assert.Contains("A descrição do produto é obrigatório.", result.Message.First());
        }

        /// Testa o retorno de erro quando a validação de negócio falha (ex: Preço negativo).
        [Fact]
        public async Task AddAsync_ValidationFailsPreco()
        {
            // Preparar
            var request = new List<ProdutoRequestDto?> { GetProdutoRequestDtoMock() };
            long usuarioId = 1;
            var produtoRequest = GetProdutoMock();
            produtoRequest.Preco = -2m;
            var produto = new List<Produto> { produtoRequest };

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map<List<Produto>>(request)).Returns(produto);

            // Agir
            var result = await _produtoHandler.AddRangeAsync(request, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.True(result.Message.Count > 0);
            Assert.Contains("O preço do produto deve ser maior que zero.", result.Message.First());
        }

        /// Testa o retorno de erro quando a validação de negócio falha (ex: Quantidade negativa).
        [Fact]
        public async Task AddAsync_ValidationFailsQuantidade()
        {
            // Preparar
            var request = new List<ProdutoRequestDto?> { GetProdutoRequestDtoMock() };
            long usuarioId = 1;
            var produtoRequest = GetProdutoMock();
            produtoRequest.Quantidade = -2;
            var produto = new List<Produto> { produtoRequest };

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map<List<Produto>>(request)).Returns(produto);

            // Agir
            var result = await _produtoHandler.AddRangeAsync(request, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.True(result.Message.Count > 0);
            Assert.Contains("A quantidade do produto não pode ser negativa.", result.Message.First());
        }


        /// Testa se o handler lança uma exceção para erros inesperados do repositório.
        [Fact]
        public async Task AddAsync_ShouldThrowException()
        {
            // Preparar
            var request = new List<ProdutoRequestDto?> { GetProdutoRequestDtoMock() };
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ThrowsAsync(new Exception("Erro inesperado"));

            // Agir & Verificar
            await Assert.ThrowsAsync<Exception>(() => _produtoHandler.AddRangeAsync(request, 1));
        }

        #endregion

        #region TESTES PARA UpdateAsync


        /// Testa o cenário de sucesso na criação de um novo estoque.
        [Fact]
        public async Task UpdateAsync_Success()
        {
            // Preparar
            var request = GetProdutoRequestDtoMock();
            long usuarioId = 1;
            var produto = GetProdutoMock();
            var estoqueResponse = GetEstoqueMock();
            var responseDto = GetProdutoResponseDtoMock();

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map<Produto>(request)).Returns(produto);
            _produtoRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(produto);
            _estoqueRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(estoqueResponse);
            _mapperMock.Setup(m => m.Map<ProdutoResponseDto>(produto)).Returns(responseDto);
            _produtoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Produto>()));

            // Agir
            var result = await _produtoHandler.UpdateAsync(request, usuarioId);

            // Verificar
            Assert.True(result.Success);
            Assert.Null(result.Data);
        }


        /// Testa o retorno de erro quando o usuário é inválido.
        [Fact]
        public async Task UpdateAsync_UserNotFound()
        {
            // Preparar
            var request =  GetProdutoRequestDtoMock();
            long usuarioId = 1;
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync((Usuario?)null);

            // Agir
            var result = await _produtoHandler.UpdateAsync(request, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Usuário não encontrado.", result.Message.First());
        }

        /// Testa o retorno de erro quando a entidade é inválida.
        [Fact]
        public async Task UpdateAsync_EntityNotFound()
        {
            // Preparar
            long usuarioId = 1;
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());

            // Agir
            var result = await _produtoHandler.UpdateAsync(null, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("O Produto não pode ser nulo.", result.Message.First());
        }

        /// Testa o retorno de erro quando o id da entidade é inválido.
        [Fact]
        public async Task UpdateAsync_EntityIdNotFound()
        {
            // Preparar
            long usuarioId = 1;
            var request = GetProdutoRequestDtoMock();
            request.Id = null;

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());

            // Agir
            var result = await _produtoHandler.UpdateAsync(request, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("O Id do Produto é obrigatório para atualização.", result.Message.First());
        }


        /// Testa o retorno de erro quando a validação de negócio falha (ex: Nome vazio).
        [Fact]
        public async Task UpdateAsync_ValidationFailsNome()
        {
            // Preparar
            var request = GetProdutoRequestDtoMock();
            long usuarioId = 1;
            var produto = GetProdutoMock();
            produto.Nome = null;

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map<Produto>(request)).Returns(produto);

            // Agir
            var result = await _produtoHandler.UpdateAsync(request, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.True(result.Message.Count > 0);
            Assert.Contains("O nome do produto é obrigatório.", result.Message.First());
        }

        /// Testa o retorno de erro quando a validação de negócio falha (ex: Descrição vazia).
        [Fact]
        public async Task UpdateAsync_ValidationFailsDescricao()
        {
            // Preparar
            var request = GetProdutoRequestDtoMock();
            long usuarioId = 1;
            var produto = GetProdutoMock();
            produto.Descricao = null;

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map<Produto>(request)).Returns(produto);

            // Agir
            var result = await _produtoHandler.UpdateAsync(request, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.True(result.Message.Count > 0);
            Assert.Contains("A descrição do produto é obrigatório.", result.Message.First());
        }

        /// Testa o retorno de erro quando a validação de negócio falha (ex: Preço negativo).
        [Fact]
        public async Task UpdateAsync_ValidationFailsPreco()
        {
            // Preparar
            var request = GetProdutoRequestDtoMock();
            long usuarioId = 1;
            var produto = GetProdutoMock();
            produto.Preco = -2m;

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map<Produto>(request)).Returns(produto);

            // Agir
            var result = await _produtoHandler.UpdateAsync(request, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.True(result.Message.Count > 0);
            Assert.Contains("O preço do produto deve ser maior que zero.", result.Message.First());
        }

        /// Testa o retorno de erro quando a validação de negócio falha (ex: Quantidade negativa).
        [Fact]
        public async Task UpdateAsync_ValidationFailsQuantidade()
        {
            // Preparar
            var request = GetProdutoRequestDtoMock();
            long usuarioId = 1;
            var produto = GetProdutoMock();
            produto.Quantidade = -1;

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _mapperMock.Setup(m => m.Map<Produto>(request)).Returns(produto);

            // Agir
            var result = await _produtoHandler.UpdateAsync(request, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.True(result.Message.Count > 0);
            Assert.Contains("A quantidade do produto não pode ser negativa.", result.Message.First());
        }


        /// Testa se o handler lança uma exceção para erros inesperados do repositório.
        [Fact]
        public async Task UpdateAsync_ShouldThrowException()
        {
            // Preparar
            var request = GetProdutoRequestDtoMock();
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ThrowsAsync(new Exception("Erro inesperado"));

            // Agir & Verificar
            await Assert.ThrowsAsync<Exception>(() => _produtoHandler.UpdateAsync(request, 1));
        }


        #endregion

        #region TESTES PARA DeleteAsync

        /// Testa o cenário de sucesso na exclusão.
        [Fact]
        public async Task DeleteAsync_Success()
        {
            // Preparar
            long produtoId = 1;
            long usuarioId = 4;
            var estoqueExistente = GetEstoqueMock();
            var produto = GetProdutoMock();

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _produtoRepositoryMock.Setup(x => x.GetByIdAsync(produtoId, usuarioId)).ReturnsAsync(produto);
            _produtoRepositoryMock.Setup(x => x.UpdateAsync(produto)).ReturnsAsync(produto);
            _mapperMock.Setup(m => m.Map<ProdutoResponseDto>(It.IsAny<Produto>())).Returns(GetProdutoResponseDtoMock());

            // Agir
            var result = await _produtoHandler.DeleteAsync(produtoId, usuarioId);

            // Verificar
            Assert.True(result.Success);
            Assert.Null(result.Data);
            Assert.Empty(result.Message ?? new List<string>());
        }


        /// Testa o retorno de erro quando o usuário da requisição não é encontrado.
        [Fact]
        public async Task DeleteAsync_UserNotFound()
        {
            // Preparar
            long produtoId = 1;
            long usuarioId = 4;
            var estoqueExistente = GetEstoqueMock();
            var produto = GetProdutoMock();

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync((Usuario?)null);

            // Agir
            var result = await _produtoHandler.DeleteAsync(produtoId, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Usuário não encontrado.", result.Message.First());
        }


        /// Testa o retorno de erro quando o produto da requisição não é encontrado.
        [Fact]
        public async Task DeleteAsync_ProdutoNotFound()
        {
            // Preparar
            long produtoId = 1;
            long usuarioId = 4;
            var estoqueExistente = GetEstoqueMock();
            var produto = GetProdutoMock();

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId)).ReturnsAsync(GetUsuarioMock());
            _produtoRepositoryMock.Setup(x => x.GetByIdAsync(produtoId, usuarioId)).ReturnsAsync((Produto?)null);

            // Agir
            var result = await _produtoHandler.DeleteAsync(produtoId, usuarioId);

            // Verificar
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Contains("Produto não encontrado para o usuário.", result.Message.First());
        }

        /// Testa se o handler lança uma exceção para erros inesperados do repositório.
        [Fact]
        public async Task DeleteAsync_ShouldThrowException()
        {
            // Preparar
            var request = GetProdutoRequestDtoMock() ;
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ThrowsAsync(new Exception("Erro inesperado"));

            // Agir & Verificar
            await Assert.ThrowsAsync<Exception>(() => _produtoHandler.UpdateAsync(request, 1));
        }


        #endregion
    }
}