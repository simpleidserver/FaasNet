using FaasNet.Application.Core.ApplicationDomain.Queries.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Application.EF.Configurations
{
    public class ApplicationDomainConfiguration : IEntityTypeConfiguration<ApplicationDomainResult>
    {
        public void Configure(EntityTypeBuilder<ApplicationDomainResult> builder)
        {
            builder.HasKey(ad => ad.Id);
        }
    }
}
