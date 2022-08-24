using FaasNet.Peer.Client.Transports;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Peer.Client
{
    public interface IClientTransportFactory
    {
        IClientTransport Create();
    }

    public class ClientTransportFactory : IClientTransportFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ClientTransportFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IClientTransport Create()
        {
            return _serviceProvider.GetService<IClientTransport>();
        }

        public static IClientTransport NewTCP()
        {
            return new ClientTCPTransport();
        }

        public static IClientTransport NewUDP()
        {
            return new ClientUDPTransport();
        }
    }
}
