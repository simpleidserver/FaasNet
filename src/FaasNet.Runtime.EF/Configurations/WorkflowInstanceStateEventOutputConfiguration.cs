using FaasNet.Runtime.Domains.Instances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowInstanceStateEventOutputConfiguration : IEntityTypeConfiguration<WorkflowInstanceStateEventOutput>
    {
        public void Configure(EntityTypeBuilder<WorkflowInstanceStateEventOutput> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Ignore(b => b.DataObj);
        }
    }
}
