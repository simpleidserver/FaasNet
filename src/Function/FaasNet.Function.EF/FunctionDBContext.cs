using FaasNet.Function.Core.Domains;
using Microsoft.EntityFrameworkCore;

namespace FaasNet.Function.EF
{
    public class FunctionDBContext : DbContext
    {
        public FunctionDBContext(DbContextOptions<FunctionDBContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<FunctionAggregate> Functions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
