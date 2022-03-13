using FaasNet.StateMachine.Runtime.Domains.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaasNet.StateMachine.Runtime.EF.Configurations
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
