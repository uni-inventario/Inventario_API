using Inventario.Core.Data;
using Inventario.Core.Models;
using Inventario.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inventario.Test.Repositories
{
    public class ProdutoRepositoryTests : BaseRepositoryTests
    {
        #region TESTES PARA GetByEstoqueIdAsync

        [Fact]
        public async Task GetByEstoqueIdAsync_DeveRetornarProdutosNaoDeletadosDoUsuario()
        {
            var context = CreateContext();

            var estoque = new Estoque
            {
                Id = 1,
                UsuarioId = 10,
                DeletedAt = null
            };

            var produtos = new List<Produto>
            {
                new Produto { Id = 1, EstoqueId = 1, Estoque = estoque, DeletedAt = null },
                new Produto { Id = 2, EstoqueId = 1, Estoque = estoque, DeletedAt = DateTime.UtcNow },
                new Produto { Id = 3, EstoqueId = 1, Estoque = estoque, DeletedAt = null }
            };

            context.Estoques.Add(estoque);
            context.Produtos.AddRange(produtos);
            context.SaveChanges();

            var repository = new ProdutoRepository(context);

            var result = await repository.GetByEstoqueIdAsync(1, 10);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Id == 1);
            Assert.Contains(result, p => p.Id == 3);
        }

        [Fact]
        public async Task GetByEstoqueIdAsync_DeveRetornarListaVaziaQuandoUsuarioNaoCorresponde()
        {
            var context = CreateContext();

            var estoque = new Estoque
            {
                Id = 1,
                UsuarioId = 10,
                DeletedAt = null
            };

            context.Estoques.Add(estoque);
            context.Produtos.Add(new Produto { Id = 1, EstoqueId = 1, Estoque = estoque, DeletedAt = null });
            context.SaveChanges();

            var repository = new ProdutoRepository(context);

            var result = await repository.GetByEstoqueIdAsync(1, 999);

            Assert.Empty(result);
        }

        #endregion

        #region TESTES PARA GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_DeveRetornarProdutoQuandoNaoDeletadoEUsuarioCorreto()
        {
            var context = CreateContext();

            var estoque = new Estoque
            {
                Id = 1,
                UsuarioId = 20,
                DeletedAt = null
            };

            var produto = new Produto
            {
                Id = 100,
                Nome = "Produto Teste",
                EstoqueId = 1,
                Estoque = estoque,
                DeletedAt = null
            };

            context.Estoques.Add(estoque);
            context.Produtos.Add(produto);
            context.SaveChanges();

            var repository = new ProdutoRepository(context);

            var result = await repository.GetByIdAsync(100, 20);

            Assert.NotNull(result);
            Assert.Equal(100, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNullQuandoProdutoEstiverDeletado()
        {
            var context = CreateContext();

            var estoque = new Estoque
            {
                Id = 1,
                UsuarioId = 20,
                DeletedAt = null
            };

            var produto = new Produto
            {
                Id = 100,
                EstoqueId = 1,
                Estoque = estoque,
                DeletedAt = DateTime.UtcNow
            };

            context.Estoques.Add(estoque);
            context.Produtos.Add(produto);
            context.SaveChanges();

            var repository = new ProdutoRepository(context);

            var result = await repository.GetByIdAsync(100, 20);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNullQuandoUsuarioNaoCorresponder()
        {
            var context = CreateContext();

            var estoque = new Estoque
            {
                Id = 1,
                UsuarioId = 20,
                DeletedAt = null
            };

            var produto = new Produto
            {
                Id = 100,
                EstoqueId = 1,
                Estoque = estoque,
                DeletedAt = null
            };

            context.Estoques.Add(estoque);
            context.Produtos.Add(produto);
            context.SaveChanges();

            var repository = new ProdutoRepository(context);

            var result = await repository.GetByIdAsync(100, 999);

            Assert.Null(result);
        }

        #endregion
    }
}
