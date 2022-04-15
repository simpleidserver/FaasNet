using FaasNet.EventMesh.Runtime.EF.Configurations;
using FaasNet.EventMesh.Runtime.MessageBroker;
using Microsoft.EntityFrameworkCore;

namespace FaasNet.EventMesh.Runtime.EF
{
    public class MessageBrokerDBContext : DbContext
    {
        public MessageBrokerDBContext(DbContextOptions<MessageBrokerDBContext> dbContextOptions) : base(dbContextOptions) { }

        public virtual DbSet<EventMeshCloudEvent> CloudEvts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new EventMeshCloudEventConfiguration());
        }
    }
}
