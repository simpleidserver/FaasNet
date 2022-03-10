using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore
{
    public abstract class BaseQueryProjection
    {
        private readonly IEventStoreConsumer _eventStoreConsumer;

        public BaseQueryProjection(IEventStoreConsumer eventStoreConsumer)
        {
            _eventStoreConsumer = eventStoreConsumer;
        }

        public void Run(CancellationToken cancellationToken)
        {
            _eventStoreConsumer.Subscribe(Topic, 0, (domainEvt) =>
            {
                return Task.CompletedTask;
            }, cancellationToken);
        }


        protected abstract string Topic { get; }
        protected abstract string GroupId { get; }
        protected abstract void Project(ProjectionBuilder builder);
    }
}