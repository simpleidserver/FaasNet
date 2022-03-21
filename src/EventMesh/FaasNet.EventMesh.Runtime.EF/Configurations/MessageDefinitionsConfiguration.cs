using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.Runtime.EF.Configurations
{
    public class MessageDefinitionsConfiguration : IEntityTypeConfiguration<MessageDefinition>
    {
        public void Configure(EntityTypeBuilder<MessageDefinition> builder)
        {
            builder.HasKey(m => m.Id);
        }
    }
}
