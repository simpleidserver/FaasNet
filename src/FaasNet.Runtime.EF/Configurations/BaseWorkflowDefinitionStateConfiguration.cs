using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class BaseWorkflowDefinitionStateConfiguration : IEntityTypeConfiguration<BaseWorkflowDefinitionState>
    {
        public void Configure(EntityTypeBuilder<BaseWorkflowDefinitionState> builder)
        {
            builder.HasKey(_ => _.TechnicalId);
            builder.HasDiscriminator(_ => _.Type)
                .HasValue<WorkflowDefinitionEventState>(WorkflowDefinitionStateTypes.Event)
                .HasValue<WorkflowDefinitionOperationState>(WorkflowDefinitionStateTypes.Operation)
                .HasValue<WorkflowDefinitionSwitchState>(WorkflowDefinitionStateTypes.Switch)
                .HasValue<WorkflowDefinitionInjectState>(WorkflowDefinitionStateTypes.Inject)
                .HasValue<WorkflowDefinitionForeachState>(WorkflowDefinitionStateTypes.ForEach)
                .HasValue<WorkflowDefinitionCallbackState>(WorkflowDefinitionStateTypes.Callback);
            builder.Ignore(_ => _.StateDataFilter);
        }
    }
}
