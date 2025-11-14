using Inventario.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Core.Data
{
    public class ContextRepository: DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        public ContextRepository(DbContextOptions<ContextRepository> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Estoques)
                .WithOne()
                .HasForeignKey(e => e.UsuarioId);
                
            modelBuilder.Entity<Estoque>()
                .HasMany(e => e.Produtos)
                .WithOne()
                .HasForeignKey(p => p.EstoqueId);
        }
    }
}