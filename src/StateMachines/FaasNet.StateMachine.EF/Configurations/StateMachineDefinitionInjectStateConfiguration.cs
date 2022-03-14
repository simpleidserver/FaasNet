using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineDefinitionInjectStateConfiguration : IEntityTypeConfiguration<StateMachineDefinitionInjectState>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionInjectState> builder)
        {
            builder.Ignore(_ => _.Data);
        }
    }
}
