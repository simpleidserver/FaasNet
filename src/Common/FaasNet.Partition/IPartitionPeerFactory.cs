using FaasNet.Peer;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Partition
{
    public interface IPartitionPeerFactory
    {
        IPeerHost Build(int port, string partitionKey, Action<IServiceCollection> callback = null);
    }
}
