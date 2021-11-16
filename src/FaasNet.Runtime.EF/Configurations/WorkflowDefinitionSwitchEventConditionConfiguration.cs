using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionSwitchEventConditionConfiguration : IEntityTypeConfiguration<WorkflowDefinitionSwitchEventCondition>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionSwitchEventCondition> builder)
        {
            builder.Property(p => p.EventDataFilter)
                .HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<WorkflowDefinitionEventDataFilter>(p));
        }
    }
}
