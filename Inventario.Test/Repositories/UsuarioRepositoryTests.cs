using Inventario.Core.Data;
using Inventario.Core.Models;
using Inventario.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inventario.Test.Repositories
{
    public class UsuarioRepositoryTests : BaseRepositoryTests
    {
        #region TESTES PARA GetByEmailAsync

        [Fact]
        public async Task GetByEmailAsync_DeveRetornarUsuarioQuandoEmailExisteENaoDeletado()
        {
            var context = CreateContext();

            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                DeletedAt = null
            };

            context.Usuarios.Add(usuario);
            context.SaveChanges();

            var repository = new UsuarioRepository(context);

            var result = await repository.GetByEmailAsync("teste@teste.com");

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetByEmailAsync_DeveRetornarNullQuandoUsuarioEstiverDeletado()
        {
            var context = CreateContext();

            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                DeletedAt = DateTime.Now
            };

            context.Usuarios.Add(usuario);
            context.SaveChanges();

            var repository = new UsuarioRepository(context);

            var result = await repository.GetByEmailAsync("teste@teste.com");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_DeveRetornarNullQuandoEmailNaoExiste()
        {
            var context = CreateContext();

            var repository = new UsuarioRepository(context);

            var result = await repository.GetByEmailAsync("naoexiste@teste.com");

            Assert.Null(result);
        }

        #endregion

        #region TESTES PARA UpdateTokenAsync

        [Fact]
        public async Task UpdateTokenAsync_DeveAtualizarTokenEUpdatedAt()
        {
            var context = CreateContext();

            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                Token = null,
                UpdatedAt = DateTime.Now.AddDays(-2),
                DeletedAt = null
            };

            context.Usuarios.Add(usuario);
            context.SaveChanges();

            var repository = new UsuarioRepository(context);

            await repository.UpdateTokenAsync(1, "TOKEN123");

            var atualizado = context.Usuarios.First(u => u.Id == 1);

            Assert.Equal("TOKEN123", atualizado.Token);
            Assert.NotNull(atualizado.UpdatedAt);
        }

        #endregion

        #region TESTES PARA CheckTokenAsync

        [Fact]
        public async Task CheckTokenAsync_DeveRetornarTrueQuandoTokenCorresponde()
        {
            var context = CreateContext();

            var usuario = new Usuario
            {
                Id = 1,
                Email = "teste@test.com",
                Token = "ABC123",
                DeletedAt = null
            };

            context.Usuarios.Add(usuario);
            context.SaveChanges();

            var repository = new UsuarioRepository(context);

            var result = await repository.CheckTokenAsync(1, "ABC123");

            Assert.True(result);
        }

        [Fact]
        public async Task CheckTokenAsync_DeveRetornarFalseQuandoTokenNaoCorresponde()
        {
            var context = CreateContext();

            var usuario = new Usuario
            {
                Id = 1,
                Email = "teste@test.com",
                Token = "TOKEN_REAL",
                DeletedAt = null
            };

            context.Usuarios.Add(usuario);
            context.SaveChanges();

            var repository = new UsuarioRepository(context);

            var result = await repository.CheckTokenAsync(1, "TOKEN_ERRADO");

            Assert.False(result);
        }

        [Fact]
        public async Task CheckTokenAsync_DeveRetornarFalseQuandoUsuarioNaoExiste()
        {
            var context = CreateContext();

            var repository = new UsuarioRepository(context);

            var result = await repository.CheckTokenAsync(999, "TOKEN");

            Assert.False(result);
        }

        [Fact]
        public async Task CheckTokenAsync_DeveRetornarFalseQuandoUsuarioEstiverDeletado()
        {
            var context = CreateContext();

            context.Usuarios.Add(new Usuario
            {
                Id = 1,
                Email = "x",
                Token = "ABC",
                DeletedAt = DateTime.Now
            });

            context.SaveChanges();

            var repository = new UsuarioRepository(context);

            var result = await repository.CheckTokenAsync(1, "ABC");

            Assert.False(result);
        }

        #endregion
    }
}
