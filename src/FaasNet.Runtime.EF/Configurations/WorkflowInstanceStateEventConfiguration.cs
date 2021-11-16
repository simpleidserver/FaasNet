using FaasNet.Runtime.Domains.Instances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowInstanceStateEventConfiguration : IEntityTypeConfiguration<WorkflowInstanceStateEvent>
    {
        public void Configure(EntityTypeBuilder<WorkflowInstanceStateEvent> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Ignore(_ => _.InputDataObj);
            builder.HasMany(_ => _.OutputLst).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
