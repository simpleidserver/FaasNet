using FaasNet.EventMesh.Core.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.EF.Configurations
{
    public class EventMeshServerBridgeConfiguration : IEntityTypeConfiguration<EventMeshServerBridge>
    {
        public void Configure(EntityTypeBuilder<EventMeshServerBridge> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
