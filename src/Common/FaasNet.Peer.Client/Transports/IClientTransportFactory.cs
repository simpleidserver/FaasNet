using FaasNet.Peer.Client.Transports;

namespace FaasNet.Peer.Client
{
    public interface IClientTransportFactory
    {
        IClientTransport Create();
    }

    public class UDPClientTransportFactory : IClientTransportFactory
    {
        public IClientTransport Create() => new ClientUDPTransport();
    }

    public class TCPClientTransportFactory : IClientTransportFactory
    {
        public IClientTransport Create() => new ClientTCPTransport();
    }
}
