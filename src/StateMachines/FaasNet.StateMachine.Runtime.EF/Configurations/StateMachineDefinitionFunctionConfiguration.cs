using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.Runtime.EF.Configurations
{
    public class StateMachineDefinitionFunctionConfiguration : IEntityTypeConfiguration<StateMachineDefinitionFunction>
    {
        public void Configure(EntityTypeBuilder<StateMachineDefinitionFunction> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Ignore(_ => _.Metadata);
        }
    }
}
