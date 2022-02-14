using EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventMesh.Runtime.EF.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.ClientId);
            builder.HasMany(c => c.Sessions).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Topics).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(c => c.ActiveSessions);
        }
    }
}
