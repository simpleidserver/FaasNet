using FaasNet.StateMachine.Runtime.Domains.Instances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.Runtime.EF.Configurations
{
    public class StateMachineInstanceStateEventOutputConfiguration : IEntityTypeConfiguration<StateMachineInstanceStateEventOutput>
    {
        public void Configure(EntityTypeBuilder<StateMachineInstanceStateEventOutput> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Ignore(b => b.DataObj);
        }
    }
}
