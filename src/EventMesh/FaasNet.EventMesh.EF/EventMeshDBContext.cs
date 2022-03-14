using FaasNet.EventMesh.Core.Domains;
using Microsoft.EntityFrameworkCore;

namespace FaasNet.EventMesh.EF
{
    public class EventMeshDBContext : DbContext
    {
        public EventMeshDBContext(DbContextOptions<EventMeshDBContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<EventMeshServerAggregate> EventMeshServers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
