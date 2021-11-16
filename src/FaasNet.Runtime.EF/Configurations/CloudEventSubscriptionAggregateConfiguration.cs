using FaasNet.Runtime.Domains.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.Runtime.EF.Configurations
{
    public class CloudEventSubscriptionAggregateConfiguration : IEntityTypeConfiguration<CloudEventSubscriptionAggregate>
    {
        public void Configure(EntityTypeBuilder<CloudEventSubscriptionAggregate> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
