using FaasNet.EventStore.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace FaasNet.Application.SqlServer.Startup
{
    public class EventStoreDBContextMigration : IDesignTimeDbContextFactory<EventStoreDBContext>
    {
        public EventStoreDBContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<EventStoreDBContext>();
            builder.UseSqlServer("Data Source=DESKTOP-F641MIJ\\SQLEXPRESS;Initial Catalog=Application;Integrated Security=True",
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new EventStoreDBContext(builder.Options);
        }
    }
}
