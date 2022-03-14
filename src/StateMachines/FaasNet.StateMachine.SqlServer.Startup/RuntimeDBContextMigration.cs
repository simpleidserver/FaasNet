using FaasNet.StateMachine.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace FaasNet.StateMachine.SqlServer.Startup
{
    public class RuntimeDBContextMigration : IDesignTimeDbContextFactory<RuntimeDBContext>
    {
        public RuntimeDBContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<RuntimeDBContext>();
            builder.UseSqlServer("Data Source=DESKTOP-F641MIJ\\SQLEXPRESS;Initial Catalog=Runtime;Integrated Security=True",
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new RuntimeDBContext(builder.Options);
        }
    }
}
