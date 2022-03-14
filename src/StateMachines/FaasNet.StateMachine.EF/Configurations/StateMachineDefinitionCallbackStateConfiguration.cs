using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineDefinitionCallbackStateConfiguration : IEntityTypeConfiguration<StateMachineDefinitionCallbackState>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionCallbackState> builder)
        {
            builder.HasOne(_ => _.Action).WithOne(_ => _.CallbackState).OnDelete(DeleteBehavior.Cascade).HasForeignKey< StateMachineDefinitionCallbackState>(_ => _.ActionId);
        }
    }
}
