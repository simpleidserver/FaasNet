using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineDefinitionActionConfiguration : IEntityTypeConfiguration<StateMachineDefinitionAction>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionAction> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Property(p => p.FunctionRef)
                .HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<StateMachineDefinitionFunctionRef>(p));
            builder.Property(p => p.ActionDataFilter)
                .HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<StateMachineDefinitionActionDataFilter>(p));
        }
    }
}
