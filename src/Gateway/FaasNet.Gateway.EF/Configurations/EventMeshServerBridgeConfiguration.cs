using FaasNet.Gateway.Core.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Gateway.EF.Configurations
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
