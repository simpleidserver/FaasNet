using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker
{
    public interface IEventConsumerStore: IDisposable
    {
        Task Init(CancellationToken cancellationToken);
        bool IsListeningVpn(string vpn);
        Task ListenVpn(string vpn, CancellationToken cancellationToken);
        void Stop();
    }
}
