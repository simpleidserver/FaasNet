using EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventMesh.Runtime.EF.Configurations
{
    public class ClientSessionBridgeConfiguration : IEntityTypeConfiguration<ClientSessionBridge>
    {
        public void Configure(EntityTypeBuilder<ClientSessionBridge> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
