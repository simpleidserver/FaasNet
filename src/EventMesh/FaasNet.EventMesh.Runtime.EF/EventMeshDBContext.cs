using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace FaasNet.EventMesh.Runtime.EF
{
    public class EventMeshDBContext : DbContext
    {
        public EventMeshDBContext(DbContextOptions<EventMeshDBContext> dbContextOptions) : base(dbContextOptions) { }


        public static SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public virtual DbSet<Vpn> VpnLst { get; set; }
        public virtual DbSet<MessageDefinition> MessageDefinitionLst { get; set; }
        public virtual DbSet<Client> ClientLst { get; set; }
        public virtual DbSet<BrokerConfiguration> BrokerConfigurations { get; set; }
        public virtual DbSet<ApplicationDomain> ApplicationDomains { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
