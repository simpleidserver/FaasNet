using FaasNet.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker
{
    public interface IIntegrationEventProcessor
    {
        Task Process(List<IntegrationEvent> integrationEvts, CancellationToken cancellationToken);
    }
}
