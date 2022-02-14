using EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventMesh.Runtime.EF.Configurations
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
