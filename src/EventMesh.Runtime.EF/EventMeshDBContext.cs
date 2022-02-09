using EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;

namespace EventMesh.Runtime.EF
{
    public class EventMeshDBContext : DbContext
    {
        public EventMeshDBContext(DbContextOptions<EventMeshDBContext> dbContextOptions) : base(dbContextOptions) { }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<BridgeServer> BridgeServers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
