using FaasNet.StateMachine.Runtime.Domains.Instances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineInstanceStateHistoryConfiguration : IEntityTypeConfiguration<StateMachineInstanceStateHistory>
    {
        public void Configure(EntityTypeBuilder<StateMachineInstanceStateHistory> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
