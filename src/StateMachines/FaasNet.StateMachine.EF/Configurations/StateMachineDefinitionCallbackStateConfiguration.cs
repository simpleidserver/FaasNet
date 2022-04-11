using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineDefinitionCallbackStateConfiguration : IEntityTypeConfiguration<StateMachineDefinitionCallbackState>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionCallbackState> builder)
        {
            builder.HasOne(_ => _.Action).WithOne(_ => _.CallbackState).OnDelete(DeleteBehavior.Cascade).HasForeignKey<StateMachineDefinitionCallbackState>(_ => _.ActionId);
            builder.Property(p => p.EventDataFilter).HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<StateMachineDefinitionEventDataFilter>(p));
        }
    }
}
