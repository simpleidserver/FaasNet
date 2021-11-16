using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Instances;
using FaasNet.Runtime.Domains.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FaasNet.Runtime.EF
{
    public class RuntimeDBContext : DbContext
    {
        public RuntimeDBContext(DbContextOptions<RuntimeDBContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<WorkflowDefinitionAggregate> WorkflowDefinitions { get; set; } 
        public DbSet<WorkflowInstanceAggregate> WorkflowInstances { get; set; }
        public DbSet<CloudEventSubscriptionAggregate> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
