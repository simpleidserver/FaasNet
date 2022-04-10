using FaasNet.EventStore;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Domains.Instances.Events;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Exporter
{
    public class StateMachineInstanceQueryProjection : BaseQueryProjection
    {
        private readonly StateMachineExporterOptions _options;

        public StateMachineInstanceQueryProjection(IEventStoreConsumer eventStoreConsumer, ISubscriptionRepository subscriptionRepository, IOptions<StateMachineExporterOptions> options) : base(eventStoreConsumer, subscriptionRepository)
        {
            _options = options.Value;
        }

        protected override string Topic => StateMachineInstanceAggregate.TOPIC_NAME;
        protected override string GroupId => _options.GroupId;

        protected override void Project(ProjectionBuilder builder)
        {
            builder.On<StateMachineInstanceCreatedEvent>((evt) =>
            {
                return Task.CompletedTask;
            });
        }
    }
}
