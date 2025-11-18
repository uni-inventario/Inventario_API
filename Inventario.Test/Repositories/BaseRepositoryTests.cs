using Inventario.Core.Data;
using Inventario.Core.Models;
using Inventario.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inventario.Test.Repositories
{
    public class BaseRepositoryTests
    {
        #region Constructor
        protected ContextRepository CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ContextRepository>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ContextRepository(options);
        }
        #endregion

        #region MOCKS AUXILIARES
        private Produto GetProdutoMock()
        {
            return new Produto
            {
                Id = 1,
                Nome = "Produto Teste",
                DeletedAt = null
            };

        }
        private Estoque GetEstoqueMock()
        {
            return new Estoque
            {
                Id = 1,
                Nome = "Teste",
                DeletedAt = null
            };

        }
        private List<Estoque> GetEstoquesMock()
        {
            return new List<Estoque>
            {
                new Estoque { Id = 1, Nome = "A", DeletedAt = null },
                new Estoque { Id = 2, Nome = "B", DeletedAt = null }
            };
        }
        private Usuario GetUsuarioMock()
        {
            return new Usuario
            {
                Id = 1,
                Nome = "teste",
                Email = "teste@exemple.com",
                DeletedAt = null
            };

        }
        private List<Produto> GetProdutosMock()
        {
            return new List<Produto>
            {
                new Produto { Id = 1, Nome = "A", DeletedAt = null },
                new Produto { Id = 2, Nome = "B", DeletedAt = null }
            };
        }
        #endregion

        #region TESTES PARA AddAsync
        // Teste do repositorio da funcionalidade AddAsync
        [Fact]
        public async Task AddAsync_DeveAdicionarEntidade()
        {
            var context = CreateContext("db_add");
            var repo = new BaseRepository<Produto>(context);

            var produto = GetProdutoMock();
            var result = await repo.AddAsync(produto);

            Assert.NotNull(result);
            Assert.Equal(1, context.Produtos.Count());
        }
        #endregion

        #region TESTES PARA AddRangeAsync
        // Teste do repositorio da funcionalidade AddRangeAsync
        [Fact]
        public async Task AddRangeAsync_DeveAdicionarMultiplasEntidades()
        {
            var context = CreateContext("db_addrange");
            var repo = new BaseRepository<Estoque>(context);

            var estoques = GetEstoquesMock();

            await repo.AddRangeAsync(estoques);

            Assert.Equal(2, context.Estoques.Count());
        }
        #endregion

        #region TESTES PARA UpdateAsync
        // Teste do repositorio da funcionalidade UpdateAsync
        [Fact]
        public async Task UpdateAsync_DeveAtualizarEntidade()
        {
            var context = CreateContext("db_update");

            var usuario = GetUsuarioMock();

            context.Usuarios.Add(usuario);
            context.SaveChanges();

            usuario.Nome = "Atualizado";

            var repo = new BaseRepository<Usuario>(context);
            var result = await repo.UpdateAsync(usuario);

            Assert.Equal("Atualizado", result.Nome);
        }
        #endregion

        #region TESTES PARA UpdateRangeAsync
        // Teste do repositorio da funcionalidade UpdateRangeAsync
        [Fact]
        public async Task UpdateRangeAsync_DeveAtualizarMultiplasEntidades()
        {
            var context = CreateContext("db_updaterange");

            var produtos = GetProdutosMock();

            context.Produtos.AddRange(produtos);
            context.SaveChanges();

            produtos[0].Nome = "A1";
            produtos[1].Nome = "B1";

            var repo = new BaseRepository<Produto>(context);
            await repo.UpdateRangeAsync(produtos);

            Assert.Equal("A1", context.Produtos.First(p => p.Id == 1).Nome);
            Assert.Equal("B1", context.Produtos.First(p => p.Id == 2).Nome);
        }
        #endregion

        #region TESTES PARA GetByIdAsync
        // Teste do repositorio da funcionalidade GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_DeveRetornarEntidade_QuandoNaoDeletada()
        {
            var context = CreateContext("db_getbyid_ok");

            var estoque = GetEstoqueMock();

            context.Estoques.Add(estoque);
            context.SaveChanges();

            var repo = new BaseRepository<Estoque>(context);
            var result = await repo.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Teste", result.Nome);
        }

        // Teste do repositorio da funcionalidade GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoDeletado()
        {
            var context = CreateContext("db_getbyid_deleted");

            var produto = GetProdutoMock();

            context.Produtos.Add(produto);
            context.SaveChanges();

            var repo = new BaseRepository<Produto>(context);
            var result = await repo.GetByIdAsync(1);

            Assert.Null(result);
        }

        // Teste do repositorio da funcionalidade GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoNaoEncontrado()
        {
            var context = CreateContext("db_getbyid_empty");
            var repo = new BaseRepository<Usuario>(context);

            var result = await repo.GetByIdAsync(999);

            Assert.Null(result);
        }
        #endregion
    }
}