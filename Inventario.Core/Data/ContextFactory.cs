using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Inventario.Core.Data
{
    public class ContextFactory : IDesignTimeDbContextFactory<ContextRepository>
    {
        public ContextRepository CreateDbContext(string[] args)
        {
            Env.Load("../.env.development");
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            var optionsBuilder = new DbContextOptionsBuilder<ContextRepository>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new ContextRepository(optionsBuilder.Options);
        }
    }
}
