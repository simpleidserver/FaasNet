using FaasNet.Application.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace FaasNet.Application.SqlServer.Startup
{
    public class ApplicationDBContextMigration : IDesignTimeDbContextFactory<ApplicationDBContext>
    {
        public ApplicationDBContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<ApplicationDBContext>();
            builder.UseSqlServer("Data Source=DESKTOP-F641MIJ\\SQLEXPRESS;Initial Catalog=Application;Integrated Security=True",
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new ApplicationDBContext(builder.Options);
        }
    }
}
