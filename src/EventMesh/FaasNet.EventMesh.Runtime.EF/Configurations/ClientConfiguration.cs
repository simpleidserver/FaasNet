using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace FaasNet.EventMesh.Runtime.EF.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.ClientId);
            builder.Property(c => c.Purposes).HasConversion(
                p => string.Join(',', p),
                p => p.Split(',', StringSplitOptions.None).Select(p => int.Parse(p)).ToList());
            builder.HasMany(c => c.Sessions).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Topics).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(c => c.ActiveSessions);
        }
    }
}
