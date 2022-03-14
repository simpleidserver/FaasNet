using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class BaseEventConditionConfiguration : IEntityTypeConfiguration<BaseEventCondition>
    {
        public void Configure(EntityTypeBuilder<BaseEventCondition> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.HasDiscriminator(_ => _.ConditionType)
                .HasValue<StateMachineDefinitionSwitchDataCondition>(StateMachineDefinitionEventConditionTypes.DATA)
                .HasValue<StateMachineDefinitionSwitchEventCondition>(StateMachineDefinitionEventConditionTypes.EVENT);
        }
    }
}
