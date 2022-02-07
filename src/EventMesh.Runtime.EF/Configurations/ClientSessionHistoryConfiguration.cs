using EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventMesh.Runtime.EF.Configurations
{
    public class ClientSessionHistoryConfiguration : IEntityTypeConfiguration<ClientSessionHistory>
    {
        public void Configure(EntityTypeBuilder<ClientSessionHistory> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
