using Amqp.Types;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public class RequestParameter
    {
        public RequestParameter(DescribedList cmd, byte[] payload, ushort channel)
        {
            Cmd = cmd;
            Payload = payload;
            Channel = channel;
        }

        public DescribedList Cmd { get; set; }
        public byte[] Payload { get; set; }
        public ushort Channel { get; set; }
    }
}
