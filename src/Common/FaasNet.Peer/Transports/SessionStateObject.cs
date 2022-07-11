using System.Net.Sockets;

namespace FaasNet.Peer.Transports
{
    public class SessionStateObject
    {
        public const int BufferSize = 1024;

        public SessionStateObject(Socket sessionSocket)
        {
            SessionSocket = sessionSocket;
        }

        public byte[] Buffer = new byte[BufferSize];
        public Socket SessionSocket { get; private set; }
    }
}
