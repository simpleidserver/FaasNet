using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionForeachStateConfiguration : IEntityTypeConfiguration<WorkflowDefinitionForeachState>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionForeachState> builder)
        {
            builder.HasMany(_ => _.Actions).WithOne().OnDelete(DeleteBehavior.NoAction);
        }
    }
}
