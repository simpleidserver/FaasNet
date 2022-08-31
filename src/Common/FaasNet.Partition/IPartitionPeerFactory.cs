using FaasNet.Peer;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Partition
{
    public interface IPartitionPeerFactory
    {
        IPeerHost Build(int port, string partitionKey, Type stateMachineType, Action<IServiceCollection> callback = null, Action<PeerHostFactory> callbackHostFactory = null);
    }
}
