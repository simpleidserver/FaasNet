using FaasNet.EventMesh.Runtime.MessageBroker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.Runtime.EF.Configurations
{
    public class EventMeshCloudEventConfiguration : IEntityTypeConfiguration<EventMeshCloudEvent>
    {
        public void Configure(EntityTypeBuilder<EventMeshCloudEvent> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Ignore(c => c.CloudEvt);
        }
    }
}
