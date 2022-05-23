using System.Net.Sockets;

namespace FaasNet.EventMesh.Protocols.AMQP
{
    public class StateObject
    {
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public Socket WorkSocket = null;
        public StateSessionObject Session = null;
    }
}
