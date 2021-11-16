using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionCallbackStateConfiguration : IEntityTypeConfiguration<WorkflowDefinitionCallbackState>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionCallbackState> builder)
        {
            builder.HasOne(_ => _.Action).WithOne(_ => _.CallbackState).OnDelete(DeleteBehavior.Cascade).HasForeignKey< WorkflowDefinitionCallbackState>(_ => _.ActionId);
        }
    }
}
