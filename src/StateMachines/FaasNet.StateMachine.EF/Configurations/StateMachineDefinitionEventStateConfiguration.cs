using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineDefinitionEventStateConfiguration : IEntityTypeConfiguration<StateMachineDefinitionEventState>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionEventState> builder)
        {
            builder.HasMany(_ => _.OnEvents).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
