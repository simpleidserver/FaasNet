using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.Runtime.EF.Configurations
{
    public class ClientSessionPendingCloudEventConfiguration : IEntityTypeConfiguration<ClientSessionPendingCloudEvent>
    {
        public void Configure(EntityTypeBuilder<ClientSessionPendingCloudEvent> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Ignore(cs => cs.Evt);
        }
    }
}
