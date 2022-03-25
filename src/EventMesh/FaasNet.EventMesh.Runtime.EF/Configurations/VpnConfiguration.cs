using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.Runtime.EF.Configurations
{
    public class VpnConfiguration : IEntityTypeConfiguration<Vpn>
    {
        public void Configure(EntityTypeBuilder<Vpn> builder)
        {
            builder.HasKey(v => v.Name);
            builder.HasMany(v => v.BridgeServers).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
