using FaasNet.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore
{
    public interface IEventStoreProducer
    {
        Task<bool> Append<T>(string topicName, T domainEvt, CancellationToken cancellationToken) where T : DomainEvent;
    }
}
