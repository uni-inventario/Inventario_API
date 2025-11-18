using Inventario.Core.Data;
using Inventario.Core.Models;
using Inventario.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inventario.Test.Repositories
{
    public class BaseRepositoryTests
    {
        #region Context

        protected ContextRepository CreateContext()
        {
            var options = new DbContextOptionsBuilder<ContextRepository>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ContextRepository(options);
        }
        
        #endregion

        #region TESTES PARA AddAsync

        [Fact]
        public async Task AddAsync_DeveAdicionarEntidade()
        {
            var context = CreateContext();
            var repository = new BaseRepository<Produto>(context);

            var produto = new Produto
            {
                Id = 1,
                Nome = "Produto Teste",
                DeletedAt = null
            };

            var result = await repository.AddAsync(produto);

            Assert.NotNull(result);
            Assert.Equal(1, context.Produtos.Count());
        }

        #endregion

        #region TESTES PARA AddRangeAsync

        [Fact]
        public async Task AddRangeAsync_DeveAdicionarMultiplasEntidades()
        {
            var context = CreateContext();
            var repository = new BaseRepository<Estoque>(context);

            var estoques = new List<Estoque>
            {
                new Estoque { Id = 1, Nome = "A", DeletedAt = null },
                new Estoque { Id = 2, Nome = "B", DeletedAt = null }
            };

            await repository.AddRangeAsync(estoques);

            Assert.Equal(2, context.Estoques.Count());
        }

        #endregion

        #region TESTES PARA UpdateAsync

        [Fact]
        public async Task UpdateAsync_DeveAtualizarEntidade()
        {
            var context = CreateContext();

            var usuario = new Usuario
            {
                Id = 1,
                Nome = "teste",
                Email = "teste@exemple.com",
                DeletedAt = null
            };

            context.Usuarios.Add(usuario);
            context.SaveChanges();

            usuario.Nome = "Atualizado";

            var repository = new BaseRepository<Usuario>(context);
            var result = await repository.UpdateAsync(usuario);

            Assert.Equal("Atualizado", result.Nome);
        }

        #endregion

        #region TESTES PARA UpdateRangeAsync

        [Fact]
        public async Task UpdateRangeAsync_DeveAtualizarMultiplasEntidades()
        {
            var context = CreateContext();

            var produtos = new List<Produto>
            {
                new Produto { Id = 1, Nome = "A", DeletedAt = null },
                new Produto { Id = 2, Nome = "B", DeletedAt = null }
            };

            context.Produtos.AddRange(produtos);
            context.SaveChanges();

            produtos[0].Nome = "A1";
            produtos[1].Nome = "B1";

            var repository = new BaseRepository<Produto>(context);
            await repository.UpdateRangeAsync(produtos);

            Assert.Equal("A1", context.Produtos.First(p => p.Id == 1).Nome);
            Assert.Equal("B1", context.Produtos.First(p => p.Id == 2).Nome);
        }

        #endregion

        #region TESTES PARA GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_DeveRetornarEntidadeQuandoNaoDeletada()
        {
            var context = CreateContext();

            var estoque = new Estoque
            {
                Id = 1,
                Nome = "Teste",
                DeletedAt = null
            };

            context.Estoques.Add(estoque);
            context.SaveChanges();

            var repository = new BaseRepository<Estoque>(context);
            var result = await repository.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Teste", result.Nome);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNullQuandoDeletado()
        {
            var context = CreateContext();

            var produto = new Produto
            {
                Id = 1,
                Nome = "Produto Teste",
                DeletedAt = DateTime.Now
            };

            context.Produtos.Add(produto);
            context.SaveChanges();

            var repository = new BaseRepository<Produto>(context);
            var result = await repository.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNullQuandoNaoEncontrado()
        {
            var context = CreateContext();
            var repository = new BaseRepository<Usuario>(context);

            var result = await repository.GetByIdAsync(999);

            Assert.Null(result);
        }

        #endregion
    }
}
