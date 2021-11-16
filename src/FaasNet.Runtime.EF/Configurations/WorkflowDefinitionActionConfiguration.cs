using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionActionConfiguration : IEntityTypeConfiguration<WorkflowDefinitionAction>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionAction> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Property(p => p.FunctionRef)
                .HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<WorkflowDefinitionFunctionRef>(p));
            builder.Property(p => p.ActionDataFilter)
                .HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<WorkflowDefinitionActionDataFilter>(p));
        }
    }
}
