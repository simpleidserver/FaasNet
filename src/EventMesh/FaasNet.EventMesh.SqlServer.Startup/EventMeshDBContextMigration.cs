using FaasNet.EventMesh.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace FaasNet.EventMesh.SqlServer.Startup
{
    public class EventMeshDBContextMigration : IDesignTimeDbContextFactory<EventMeshDBContext>
    {
        public EventMeshDBContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<EventMeshDBContext>();
            builder.UseSqlServer("Data Source=DESKTOP-F641MIJ\\SQLEXPRESS;Initial Catalog=EventMesh;Integrated Security=True",
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new EventMeshDBContext(builder.Options);
        }
    }
}
