using FaasNet.Gateway.Core.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Gateway.EF.Configurations
{
    public class FunctionAggregateConfiguration : IEntityTypeConfiguration<FunctionAggregate>
    {
        public void Configure(EntityTypeBuilder<FunctionAggregate> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}
