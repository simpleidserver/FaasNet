using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionAggregateConfiguration : IEntityTypeConfiguration<WorkflowDefinitionAggregate>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionAggregate> builder)
        {
            builder.HasKey(_ => _.Id);
            builder.Property(p => p.Start)
                .HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<WorkflowDefinitionStartState>(p));
            builder.HasMany(_ => _.States).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(_ => _.Functions).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(_ => _.Events).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
