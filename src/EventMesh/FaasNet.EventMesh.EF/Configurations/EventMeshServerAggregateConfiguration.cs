using FaasNet.EventMesh.Core.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.EF.Configurations
{
    public class EventMeshServerAggregateConfiguration : IEntityTypeConfiguration<EventMeshServerAggregate>
    {
        public void Configure(EntityTypeBuilder<EventMeshServerAggregate> builder)
        {
            builder.HasKey(e => new { e.Urn, e.Port });
            builder.HasMany(e => e.Bridges).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
