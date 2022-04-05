using CloudNative.CloudEvents;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker
{
    public interface IMessageListener
    {
        string Name { get; }
        bool SupportVpn { get; }
        Task<IMessageListenerResult> Listen(string vpn, Action<MessageResult> callback, CancellationToken cancellationToken);
    }

    public interface IMessageListenerResult
    {
        void Stop();
    }

    public class MessageResult
    {
        public string Vpn { get; set; }
        public ICollection<CloudEvent> Content { get; set; }
    }
}
