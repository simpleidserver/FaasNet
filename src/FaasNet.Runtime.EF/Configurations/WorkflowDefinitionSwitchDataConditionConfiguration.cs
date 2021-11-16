using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionSwitchDataConditionConfiguration : IEntityTypeConfiguration<WorkflowDefinitionSwitchDataCondition>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionSwitchDataCondition> builder)
        {

        }
    }
}
