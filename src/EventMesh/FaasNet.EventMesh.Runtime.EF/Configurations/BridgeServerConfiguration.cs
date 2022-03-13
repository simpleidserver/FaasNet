using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.Runtime.EF.Configurations
{
    public class BridgeServerConfiguration : IEntityTypeConfiguration<BridgeServer>
    {
        public void Configure(EntityTypeBuilder<BridgeServer> builder)
        {
            builder.HasKey(b => b.Urn);
        }
    }
}
