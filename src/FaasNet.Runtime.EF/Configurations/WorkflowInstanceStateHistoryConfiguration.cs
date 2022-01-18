using FaasNet.Runtime.Domains.Instances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowInstanceStateHistoryConfiguration : IEntityTypeConfiguration<WorkflowInstanceStateHistory>
    {
        public void Configure(EntityTypeBuilder<WorkflowInstanceStateHistory> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
