using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionEventConfiguration : IEntityTypeConfiguration<WorkflowDefinitionEvent>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionEvent> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
