using FaasNet.EventMesh.Protocols.AMQP.Framing;
using System.Net;
using System.Net.Sockets;

namespace FaasNet.EventMesh.Protocols.AMQP
{
    public class AMQPServer
    {
        private Socket _server;

        public AMQPServer()
        {

        }

        public void Start()
        {
            Launch();
            Listen();
        }

        private void Launch()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Loopback, 5672);
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(localEndPoint);
            _server.Listen();
        }

        private void Listen()
        {
            var handler = _server.Accept();
            // Header.
            var header = ReadHeader(handler);
            handler.Send(header.Serialize());
            // Frames
            var frame = new Frame { Channel = 0, Type = FrameTypes.Amqp };
            handler.Send(frame.Serialize());
        }

        private ProtocolHeader ReadHeader(Socket socket)
        {
            var headerBuffer = new byte[FixedWidth.ULong];
            socket.Receive(headerBuffer, 0, FixedWidth.ULong, SocketFlags.None);
            var receivedHeader = ProtocolHeader.Create(headerBuffer, 0);
            return receivedHeader;
        }
    }
}
