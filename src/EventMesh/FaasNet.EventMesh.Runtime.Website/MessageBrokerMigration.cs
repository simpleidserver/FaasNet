using FaasNet.EventMesh.Runtime.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace FaasNet.EventMesh.Runtime.Website
{
    public class MessageBrokerMigration : IDesignTimeDbContextFactory<MessageBrokerDBContext>
    {
        public MessageBrokerDBContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<MessageBrokerDBContext>();
            builder.UseSqlServer("Data Source=THABART;Initial Catalog=EventMesh;Integrated Security=True", optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new MessageBrokerDBContext(builder.Options);
        }
    }
}
