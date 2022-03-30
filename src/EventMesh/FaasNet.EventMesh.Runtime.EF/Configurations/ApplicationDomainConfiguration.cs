using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.Runtime.EF.Configurations
{
    public class ApplicationDomainConfiguration : IEntityTypeConfiguration<ApplicationDomain>
    {
        public void Configure(EntityTypeBuilder<ApplicationDomain> builder)
        {
            builder.HasKey(b => b.Id);
            builder.HasMany(v => v.Applications).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(_ => _.DomainEvts);
            builder.Ignore(_ => _.IntegrationEvents);
        }
    }
}
