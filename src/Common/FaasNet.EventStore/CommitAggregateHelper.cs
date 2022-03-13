using FaasNet.Domain;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore
{
    public class CommitAggregateHelper : ICommitAggregateHelper
    {
        private readonly IEventStoreProducer _eventStoreProducer;
        private readonly IEventStoreConsumer _eventStoreConsumer;
        private readonly IEventStoreSnapshotRepository _eventStoreSnapshotRepository;
        private readonly EventStoreOptions _options;

        public CommitAggregateHelper(
            IEventStoreProducer eventStoreProducer, 
            IEventStoreConsumer eventStoreConsumer, 
            IOptions<EventStoreOptions> options,
            IEventStoreSnapshotRepository eventStoreSnapshotRepository)
        {
            _eventStoreProducer = eventStoreProducer;
            _eventStoreConsumer = eventStoreConsumer;
            _eventStoreSnapshotRepository = eventStoreSnapshotRepository;
            _options = options.Value;
        }

        public async Task Commit<T>(T domain, CancellationToken cancellationToken) where T : AggregateRoot
        {
            domain.Commit();
            foreach(var evt in domain.DomainEvts)
            {
                await _eventStoreProducer.Append(domain.Topic, evt, cancellationToken);
            }

            if(domain.Version % _options.SnapshotFrequency == 0)
            {
                await _eventStoreSnapshotRepository.Add(domain, cancellationToken);
            }
        }

        public async Task<T> Get<T>(string aggregateId, CancellationToken cancellationToken) where T : AggregateRoot
        {
            var result = await _eventStoreSnapshotRepository.GetLatest<T>(aggregateId, cancellationToken);
            var offset = 0;
            if (result == null)
            {
                result = Activator.CreateInstance<T>();
            }
            else
            {
                offset = result.LastEvtOffset;
            }

            var evts = await _eventStoreConsumer.Search(result.Topic, offset, cancellationToken);
            foreach(var evt in evts)
            {
                result.Handle(evt);
            }

            return result;
        }
    }
}
