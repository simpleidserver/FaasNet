using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore
{
    public abstract class BaseQueryProjection : IQueryProjection
    {
        private readonly IEventStoreConsumer _eventStoreConsumer;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private IDisposable _subscribe;

        public BaseQueryProjection(IEventStoreConsumer eventStoreConsumer, ISubscriptionRepository subscriptionRepository)
        {
            _eventStoreConsumer = eventStoreConsumer;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            var offset = await GetOffset(cancellationToken);
            _subscribe = await _eventStoreConsumer.Subscribe(Topic, offset, async(domainEvt) =>
            {
                var builder = new ProjectionBuilder();
                Project(builder);
                var fn = builder.Get(domainEvt.GetType());
                if (fn != null)
                {
                    await fn(domainEvt);
                }

                if (offset == null)
                {
                    offset = 0;
                }
                else
                {
                    offset++;
                }

                await Commit(offset.Value, cancellationToken);
            }, cancellationToken);
        }

        public void Stop()
        {
            _subscribe.Dispose();
        }


        protected abstract string Topic { get; }
        protected abstract string GroupId { get; }
        protected abstract void Project(ProjectionBuilder builder);

        protected virtual Task<long?> GetOffset(CancellationToken cancellationToken)
        {
            if (_eventStoreConsumer.IsOffsetSupported)
            {
                return _eventStoreConsumer.GetCurrentOffset(GroupId, Topic, cancellationToken);
            }

            return _subscriptionRepository.GetCurrentOffset(GroupId, Topic, cancellationToken);
        }

        protected virtual Task Commit(long offset, CancellationToken cancellationToken)
        {
            if (_eventStoreConsumer.IsOffsetSupported)
            {
                return _eventStoreConsumer.Commit(GroupId, Topic, offset, cancellationToken);
            }

            return _subscriptionRepository.Commit(GroupId, Topic, offset, cancellationToken);
        }
    }
}