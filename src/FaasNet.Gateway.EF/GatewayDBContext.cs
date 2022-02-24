using FaasNet.Gateway.Core.Domains;
using Microsoft.EntityFrameworkCore;

namespace FaasNet.Gateway.EF
{
    public class GatewayDBContext : DbContext
    {
        public GatewayDBContext(DbContextOptions<GatewayDBContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<FunctionAggregate> Functions { get; set; } 
        public DbSet<EventMeshServerAggregate> EventMeshServers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
