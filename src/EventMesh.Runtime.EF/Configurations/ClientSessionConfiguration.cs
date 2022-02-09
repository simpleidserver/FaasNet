using EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventMesh.Runtime.EF.Configurations
{
    public class ClientSessionConfiguration : IEntityTypeConfiguration<ClientSession>
    {
        public void Configure(EntityTypeBuilder<ClientSession> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Ignore(cs => cs.Endpoint);
            builder.Ignore(cs => cs.Purpose);
            builder.HasMany(cs => cs.Histories).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(cs => cs.Topics).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(cs => cs.PendingCloudEvents).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(cs => cs.Bridges).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
