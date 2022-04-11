using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineDefinitionEventConfiguration : IEntityTypeConfiguration<StateMachineDefinitionEvent>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionEvent> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Ignore(_ => _.Metadata);
            builder.Ignore(_ => _.Topic);
        }
    }
}
