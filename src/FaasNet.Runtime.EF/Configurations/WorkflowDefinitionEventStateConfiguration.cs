using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionEventStateConfiguration : IEntityTypeConfiguration<WorkflowDefinitionEventState>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionEventState> builder)
        {
            builder.HasMany(_ => _.OnEvents).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
