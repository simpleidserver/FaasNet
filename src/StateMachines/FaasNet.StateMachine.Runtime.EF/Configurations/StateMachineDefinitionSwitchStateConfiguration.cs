using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.Runtime.EF.Configurations
{
    public class StateMachineDefinitionSwitchStateConfiguration : IEntityTypeConfiguration<StateMachineDefinitionSwitchState>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionSwitchState> builder)
        {
            builder.HasMany(_ => _.Conditions).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(_ => _.DataConditions);
            builder.Ignore(_ => _.EventConditions);
            builder.Ignore(_ => _.DefaultCondition);
        }
    }
}
