using FaasNet.EventStore.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FaasNet.EventStore.EF
{
    public class EventStoreDBContext : DbContext
    {
        public EventStoreDBContext(DbContextOptions<EventStoreDBContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Snapshot> Snapshots { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
