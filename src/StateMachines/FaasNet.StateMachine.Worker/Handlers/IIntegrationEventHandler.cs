using FaasNet.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker.Handlers
{
    public interface IIntegrationEventHandler<T> where T : IntegrationEvent
    {
        Task Process(T evt, CancellationToken cancellationToken);
    }
}
