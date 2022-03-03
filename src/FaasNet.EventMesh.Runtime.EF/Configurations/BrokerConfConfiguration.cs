using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.Runtime.EF.Configurations
{
    public class BrokerConfConfiguration : IEntityTypeConfiguration<BrokerConfiguration>
    {
        public void Configure(EntityTypeBuilder<BrokerConfiguration> builder)
        {
            builder.HasKey(b => b.Name);
            builder.HasMany(b => b.Records).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
