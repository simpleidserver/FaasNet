using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionFunctionConfiguration : IEntityTypeConfiguration<WorkflowDefinitionFunction>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionFunction> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Ignore(_ => _.Metadata);
        }
    }
}
