using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Domains.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FaasNet.StateMachine.Runtime.EF
{
    public class RuntimeDBContext : DbContext
    {
        public RuntimeDBContext(DbContextOptions<RuntimeDBContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<StateMachineDefinitionAggregate> WorkflowDefinitions { get; set; } 
        public DbSet<StateMachineInstanceAggregate> WorkflowInstances { get; set; }
        public DbSet<CloudEventSubscriptionAggregate> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
