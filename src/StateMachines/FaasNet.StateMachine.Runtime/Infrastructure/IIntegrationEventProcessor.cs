using FaasNet.StateMachine.Runtime.Domains.IntegrationEvents;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Infrastructure
{
    public interface IIntegrationEventProcessor
    {
        Task Process(List<IntegrationEvent> integrationEvts, CancellationToken cancellationToken);
    }
}
