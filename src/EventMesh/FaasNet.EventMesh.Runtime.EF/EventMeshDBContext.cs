using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;

namespace FaasNet.EventMesh.Runtime.EF
{
    public class EventMeshDBContext : DbContext
    {
        public EventMeshDBContext(DbContextOptions<EventMeshDBContext> dbContextOptions) : base(dbContextOptions) { }


        public static object Lock = new object();

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<BridgeServer> BridgeServers { get; set; }
        public virtual DbSet<BrokerConfiguration> BrokerConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
