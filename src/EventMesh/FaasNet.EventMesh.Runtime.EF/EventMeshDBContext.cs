using FaasNet.EventMesh.Runtime.EF.Configurations;
using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace FaasNet.EventMesh.Runtime.EF
{
    public class EventMeshDBContext : DbContext
    {
        public EventMeshDBContext(DbContextOptions<EventMeshDBContext> dbContextOptions) : base(dbContextOptions)
        {
        }


        public static SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public virtual DbSet<Vpn> VpnLst { get; set; }
        public virtual DbSet<MessageDefinition> MessageDefinitionLst { get; set; }
        public virtual DbSet<Models.Client> ClientLst { get; set; }
        public virtual DbSet<BrokerConfiguration> BrokerConfigurations { get; set; }
        public virtual DbSet<ApplicationDomain> ApplicationDomains { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationDomainConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationLinkConfiguration());
            modelBuilder.ApplyConfiguration(new BridgeServerConfiguration());
            modelBuilder.ApplyConfiguration(new BrokerConfConfiguration());
            modelBuilder.ApplyConfiguration(new BrokerConfigurationRecordConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new ClientSessionBridgeConfiguration());
            modelBuilder.ApplyConfiguration(new ClientSessionConfiguration());
            modelBuilder.ApplyConfiguration(new ClientSessionHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ClientSessionPendingCloudEventConfiguration());
            modelBuilder.ApplyConfiguration(new ClientTopicConfiguration());
            modelBuilder.ApplyConfiguration(new MessageDefinitionsConfiguration());
            modelBuilder.ApplyConfiguration(new TopicConfiguration());
            modelBuilder.ApplyConfiguration(new VpnConfiguration());
        }
    }
}
