using FaasNet.Runtime.Domains.Instances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowInstanceStateConfiguration : IEntityTypeConfiguration<WorkflowInstanceState>
    {
        public void Configure(EntityTypeBuilder<WorkflowInstanceState> builder)
        {
            builder.HasKey(_ => _.Id);
            builder.Ignore(_ => _.Input);
            builder.Ignore(_ => _.Output);
            builder.HasMany(_ => _.Events).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
