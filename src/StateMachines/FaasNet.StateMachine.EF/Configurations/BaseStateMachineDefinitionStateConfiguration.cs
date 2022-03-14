using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class BaseStateMachineDefinitionStateConfiguration : IEntityTypeConfiguration<BaseStateMachineDefinitionState>
    {
        public void Configure(EntityTypeBuilder<BaseStateMachineDefinitionState> builder)
        {
            builder.HasKey(_ => _.TechnicalId);
            builder.HasDiscriminator(_ => _.Type)
                .HasValue<StateMachineDefinitionEventState>(StateMachineDefinitionStateTypes.Event)
                .HasValue<StateMachineDefinitionOperationState>(StateMachineDefinitionStateTypes.Operation)
                .HasValue<StateMachineDefinitionSwitchState>(StateMachineDefinitionStateTypes.Switch)
                .HasValue<StateMachineDefinitionInjectState>(StateMachineDefinitionStateTypes.Inject)
                .HasValue<StateMachineDefinitionForeachState>(StateMachineDefinitionStateTypes.ForEach)
                .HasValue<StateMachineDefinitionCallbackState>(StateMachineDefinitionStateTypes.Callback);
            builder.Ignore(_ => _.StateDataFilter);
        }
    }
}
