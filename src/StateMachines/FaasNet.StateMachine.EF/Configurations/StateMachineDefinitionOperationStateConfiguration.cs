using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineDefinitionOperationStateConfiguration : IEntityTypeConfiguration<StateMachineDefinitionOperationState>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionOperationState> builder)
        {
            builder.HasMany(_ => _.Actions).WithOne().OnDelete(DeleteBehavior.NoAction);
        }
    }
}
