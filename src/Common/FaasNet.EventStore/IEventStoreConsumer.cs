using FaasNet.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore
{
    public interface IEventStoreConsumer
    {
        Task<List<DomainEvent>> Search(string topicName, long? offset, CancellationToken cancellationToken);
        Task<IDisposable> Subscribe(string topicName, long? offset, Func<DomainEvent, Task> callback, CancellationToken cancellationToken);
        bool IsOffsetSupported { get; }
        Task Commit(string groupId, string topicName, long offset, CancellationToken cancellationToken);
        Task<long?> GetCurrentOffset(string groupId, string topicName, CancellationToken cancellationToken);
    }
}
