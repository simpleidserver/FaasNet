using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionOperationStateConfiguration : IEntityTypeConfiguration<WorkflowDefinitionOperationState>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionOperationState> builder)
        {
            builder.HasMany(_ => _.Actions).WithOne().OnDelete(DeleteBehavior.NoAction);
        }
    }
}
