using FaasNet.Runtime.Domains.IntegrationEvents;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Infrastructure
{
    public interface IIntegrationEventProcessor
    {
        Task Process(List<IntegrationEvent> integrationEvts, CancellationToken cancellationToken);
    }
}
