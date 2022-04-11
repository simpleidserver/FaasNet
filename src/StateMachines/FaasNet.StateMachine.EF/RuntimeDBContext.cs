using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;

namespace FaasNet.StateMachine.EF
{
    public class RuntimeDBContext : DbContext
    {
        public RuntimeDBContext(DbContextOptions<RuntimeDBContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<StateMachineDefinitionAggregate> WorkflowDefinitions { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
