using Amqp;
using Amqp.Sasl;
using Amqp.Types;
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
            var saslHeaderNegotiation = new ProtocolHeader { Id = 3, Major = 1, Minor = 0, Revision = 0 };
            var handler = _server.Accept();
            // Check header & protocol.
            ReadHeader(handler);
            handler.Send(saslHeaderNegotiation.Serialize(), SocketFlags.None);
            // Return supported authentication mechanisms.
            var saslFrame = new Frame { Channel = 0, Type = FrameTypes.Sasl };
            var cmd = new SaslMechanisms 
            { 
                SaslServerMechanisms = new Amqp.Types.Symbol[] 
                {
                    new Amqp.Types.Symbol("PLAIN")
                }
            };
            Send(handler, saslFrame.Serialize(cmd));
            // Authenticate the user.
            ReadFrame(handler);
            var sCmd = new SaslOutcome { Code = SaslCode.Ok };
            Send(handler, saslFrame.Serialize(sCmd));
            // Open
            var saslSecuredConnection = new ProtocolHeader { Id = 0, Major = 1, Minor = 0, Revision = 0 };
            handler.Send(saslSecuredConnection.Serialize(), SocketFlags.None);
            var amqpFrame = new Frame { Channel = 0, Type = FrameTypes.Amqp };
            var tCmd = new Amqp.Framing.Open
            {
                HostName = "localhost"
            };
            Send(handler, amqpFrame.Serialize(tCmd));
            string s = "";
        }

        private ProtocolHeader ReadHeader(Socket socket)
        {
            var headerBuffer = new byte[FixedWidth.ULong];
            socket.Receive(headerBuffer, 0, FixedWidth.ULong, SocketFlags.None);
            var receivedHeader = ProtocolHeader.Create(headerBuffer, 0);
            return receivedHeader;
        }

        private Sasl.SaslPlainProfile ReadSaslPlainProfile()
        {
            return null;
        }

        private static void Send(Socket socket, ByteBuffer buffer)
        {
            socket.Send(buffer.Buffer, buffer.Offset, buffer.Length, SocketFlags.None);
        }

        private static DescribedList ReadFrame(Socket socket)
        {
            var sizeFrameBuffer = new byte[FixedWidth.UInt];
            socket.Receive(sizeFrameBuffer, 0, FixedWidth.UInt, SocketFlags.None);
            int size = AmqpBitConverter.ReadInt(sizeFrameBuffer, 0);
            var frameBuffer = new ByteBuffer(size, false);
            AmqpBitConverter.WriteInt(frameBuffer, size);
            socket.Receive(frameBuffer.Buffer, frameBuffer.Length, frameBuffer.Size, SocketFlags.None);
            frameBuffer.Append(frameBuffer.Size);
            ushort channel;
            DescribedList command;
            Frame.Decode(frameBuffer, out channel, out command);
            return command;
        }
    }
}
