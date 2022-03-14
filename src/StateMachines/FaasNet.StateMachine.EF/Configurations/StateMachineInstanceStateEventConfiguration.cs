using FaasNet.StateMachine.Runtime.Domains.Instances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineInstanceStateEventConfiguration : IEntityTypeConfiguration<StateMachineInstanceStateEvent>
    {
        public void Configure(EntityTypeBuilder<StateMachineInstanceStateEvent> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Ignore(_ => _.InputDataObj);
            builder.HasMany(_ => _.OutputLst).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
