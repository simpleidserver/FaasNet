using FaasNet.Runtime.Domains.IntegrationEvents;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Infrastructure
{
    public interface IIntegrationEventHandler<T> where T : IntegrationEvent
    {
        Task Process(T evt, CancellationToken cancellationToken);
    }
}
