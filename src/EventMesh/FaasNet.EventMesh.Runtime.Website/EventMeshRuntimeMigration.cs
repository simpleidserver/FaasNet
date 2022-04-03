using FaasNet.EventMesh.Runtime.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace FaasNet.EventMesh.Runtime.Website
{
    public class EventMeshRuntimeMigration : IDesignTimeDbContextFactory<EventMeshDBContext>
    {
        public EventMeshDBContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<EventMeshDBContext>();
            builder.UseSqlServer("Data Source=THABART;Initial Catalog=EventMesh;Integrated Security=True", optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new EventMeshDBContext(builder.Options);
        }
    }
}
