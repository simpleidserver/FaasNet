using FaasNet.StateMachine.Runtime.Domains.Instances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.EF.Configurations
{
    public class StateMachineInstanceStateConfiguration : IEntityTypeConfiguration<StateMachineInstanceState>
    {
        public void Configure(EntityTypeBuilder<StateMachineInstanceState> builder)
        {
            builder.HasKey(_ => _.Id);
            builder.Ignore(_ => _.Input);
            builder.Ignore(_ => _.Output);
            builder.HasMany(_ => _.Events).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
