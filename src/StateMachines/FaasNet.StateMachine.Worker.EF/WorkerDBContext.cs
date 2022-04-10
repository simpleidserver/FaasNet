using FaasNet.StateMachine.Worker.Domains;
using Microsoft.EntityFrameworkCore;

namespace FaasNet.StateMachine.Worker.EF
{
    public class WorkerDBContext : DbContext
    {
        public WorkerDBContext(DbContextOptions<WorkerDBContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<CloudEventSubscriptionAggregate> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
