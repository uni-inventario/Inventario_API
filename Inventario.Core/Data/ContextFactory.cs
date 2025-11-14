using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Inventario.Core.Data
{
    public class ContextFactory : IDesignTimeDbContextFactory<ContextRepository>
    {
        public ContextRepository CreateDbContext(string[] args)
        {
            var connectionString = "Server=127.0.0.1;Port=3306;Database=inventario;User=root;";

            var optionsBuilder = new DbContextOptionsBuilder<ContextRepository>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new ContextRepository(optionsBuilder.Options);
        }
    }
}
