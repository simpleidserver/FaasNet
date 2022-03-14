using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineDefinitionSwitchEventConditionConfiguration : IEntityTypeConfiguration<StateMachineDefinitionSwitchEventCondition>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionSwitchEventCondition> builder)
        {
            builder.Property(p => p.EventDataFilter)
                .HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<StateMachineDefinitionEventDataFilter>(p));
        }
    }
}
