using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionSwitchStateConfiguration : IEntityTypeConfiguration<WorkflowDefinitionSwitchState>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionSwitchState> builder)
        {
            builder.HasMany(_ => _.Conditions).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
