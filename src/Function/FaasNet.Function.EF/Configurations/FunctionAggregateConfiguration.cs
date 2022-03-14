using FaasNet.Function.Core.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.EF.Configurations
{
    public class FunctionAggregateConfiguration : IEntityTypeConfiguration<FunctionAggregate>
    {
        public void Configure(EntityTypeBuilder<FunctionAggregate> builder)
        {
            builder.HasKey(f => f.Id);
        }
    }
}
