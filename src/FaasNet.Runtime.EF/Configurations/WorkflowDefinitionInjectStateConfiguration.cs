using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionInjectStateConfiguration : IEntityTypeConfiguration<WorkflowDefinitionInjectState>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionInjectState> builder)
        {
            builder.Ignore(_ => _.Data);
        }
    }
}
