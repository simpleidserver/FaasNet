using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.Runtime.EF.Configurations
{
    public class StateMachineDefinitionSwitchDataConditionConfiguration : IEntityTypeConfiguration<StateMachineDefinitionSwitchDataCondition>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionSwitchDataCondition> builder)
        {

        }
    }
}
