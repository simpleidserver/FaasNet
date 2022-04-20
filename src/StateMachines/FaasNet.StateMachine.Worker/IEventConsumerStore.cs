using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker
{
    public interface IEventConsumerStore: IDisposable
    {
        Task Init(CancellationToken cancellationToken);
        Task Stop(CancellationToken cancellationToken);
    }
}
