using Inventario.Core.Data;
using Inventario.Core.Models;
using Inventario.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inventario.Test.Repositories
{
    public class EstoqueRepositoryTests : BaseRepositoryTests
    {
        #region TESTES PARA GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_DeveRetornarEstoqueComProdutosNaoDeletados()
        {
            var context = CreateContext();

            var estoque = new Estoque
            {
                Id = 1,
                UsuarioId = 10,
                DeletedAt = null,
                Produtos = new List<Produto>
                {
                    new Produto { Id = 1, DeletedAt = null },
                    new Produto { Id = 2, DeletedAt = DateTime.UtcNow }
                }
            };

            context.Estoques.Add(estoque);
            context.SaveChanges();

            var repository = new EstoqueRepository(context);

            var result = await repository.GetByIdAsync(1, 10);

            Assert.NotNull(result);
            Assert.Single(result.Produtos);
            Assert.Equal(1, result.Produtos.First().Id);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNullQuandoNaoEncontrar()
        {
            var context = CreateContext();
            var repository = new EstoqueRepository(context);

            var result = await repository.GetByIdAsync(999, 10);

            Assert.Null(result);
        }

        #endregion

        #region TESTES PARA GetAllAsync

        [Fact]
        public async Task GetAllAsync_DeveRetornarApenasEstoquesEProdutosNaoDeletados()
        {
            var context = CreateContext();

            var estoques = new List<Estoque>
            {
                new Estoque
                {
                    Id = 1,
                    UsuarioId = 5,
                    DeletedAt = null,
                    Produtos = new List<Produto>
                    {
                        new Produto { Id = 1, DeletedAt = null },
                        new Produto { Id = 2, DeletedAt = DateTime.UtcNow }
                    }
                },
                new Estoque
                {
                    Id = 2,
                    UsuarioId = 5,
                    DeletedAt = DateTime.UtcNow,
                    Produtos = new List<Produto>()
                },
                new Estoque
                {
                    Id = 3,
                    UsuarioId = 99,
                    DeletedAt = null,
                    Produtos = new List<Produto>()
                }
            };

            context.Estoques.AddRange(estoques);
            context.SaveChanges();

            var repository = new EstoqueRepository(context);

            var result = await repository.GetAllAsync(5);

            Assert.Single(result);
            Assert.Single(result[0].Produtos);
            Assert.Equal(1, result[0].Produtos.First().Id);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaVaziaQuandoNaoHouverEstoques()
        {
            var context = CreateContext();

            var repository = new EstoqueRepository(context);

            var result = await repository.GetAllAsync(55);

            Assert.Empty(result);
        }

        #endregion
    }
}
