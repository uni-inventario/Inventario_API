using Inventario.Core.Data;
using Inventario.Core.Models;
using Inventario.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inventario.Test.Repositories
{
    public class EstoqueRepositoryTests : BaseRepositoryTests
    {
        #region MOCKS AUXILIARES
        private Estoque getEstoqueMock()
        {
            return new Estoque
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

        }
        private List<Estoque> getEstoquesMock()
        {
            return new List<Estoque>
            {
                new() {
                    Id = 1,
                    UsuarioId = 5,
                    DeletedAt = null,
                    Produtos = new List<Produto>
                    {
                        new Produto { Id = 1, DeletedAt = null },
                        new Produto { Id = 2, DeletedAt = DateTime.UtcNow }
                    }
                },
                new() {
                    Id = 2,
                    UsuarioId = 5,
                    DeletedAt = DateTime.UtcNow,
                    Produtos = new List<Produto>()
                },
                new() {
                    Id = 3,
                    UsuarioId = 99,
                    DeletedAt = null
                }
            };
        }
        #endregion
        #region TESTES PARA GetByIdAsync
        // Teste do repositorio da funcionalidade GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_DeveRetornarEstoqueComProdutosNaoDeletados()
        {
            var context = CreateContext("db_getbyid");
            var estoque = getEstoqueMock();

            context.Estoques.Add(estoque);
            context.SaveChanges();

            var repository = new EstoqueRepository(context);

            var result = await repository.GetByIdAsync(1, 10);

            Assert.NotNull(result);
            Assert.Single(result.Produtos);
            Assert.Equal(1, result.Produtos.First().Id);
        }

        // Teste do repositorio da funcionalidade GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoNaoEncontrar()
        {
            var context = CreateContext("db_getbyid_empty");
            var repository = new EstoqueRepository(context);

            var result = await repository.GetByIdAsync(999, 10);

            Assert.Null(result);
        }
        #endregion

        #region TESTES PARA GetAllAsync
        // Teste do repositorio da funcionalidade GetAllAsync
        [Fact]
        public async Task GetAllAsync_DeveRetornarApenasEstoquesEProdutosNaoDeletados()
        {
            var context = CreateContext("db_getall");

            context.Estoques.AddRange(getEstoquesMock());

            context.SaveChanges();

            var repository = new EstoqueRepository(context);

            var result = await repository.GetAllAsync(5);

            Assert.Single(result);
            Assert.Single(result[0].Produtos);
            Assert.Equal(1, result[0].Produtos.First().Id);
        }

        // Teste do repositorio da funcionalidade GetAllAsync
        [Fact]
        public async Task GetAllAsync_DeveRetornarListaVazia_QuandoNaoHouverEstoques()
        {
            var context = CreateContext("db_getall_empty");

            var repository = new EstoqueRepository(context);

            var result = await repository.GetAllAsync(55);

            Assert.Empty(result);
        }
        #endregion
    }
}