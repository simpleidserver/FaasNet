using FaasNet.EventMesh.Runtime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.EventMesh.Runtime.EF.Configurations
{
    public class BrokerConfigurationRecordConfiguration : IEntityTypeConfiguration<BrokerConfigurationRecord>
    {
        public void Configure(EntityTypeBuilder<BrokerConfigurationRecord> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
