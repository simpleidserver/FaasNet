using System.Net.Sockets;

namespace EventMesh.Runtime
{
    public interface IUdpClientServerFactory
    {
        UdpClient Build();
    }
}
