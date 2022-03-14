using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineDefinitionForeachStateConfiguration : IEntityTypeConfiguration<StateMachineDefinitionForeachState>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionForeachState> builder)
        {
            builder.HasMany(_ => _.Actions).WithOne().OnDelete(DeleteBehavior.NoAction);
        }
    }
}
