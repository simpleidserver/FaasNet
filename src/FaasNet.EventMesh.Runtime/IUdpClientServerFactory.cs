using System.Net.Sockets;

namespace FaasNet.EventMesh.Runtime
{
    public interface IUdpClientServerFactory
    {
        UdpClient Build();
    }
}
