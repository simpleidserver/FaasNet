using System;
using System.Text;

namespace FaasNet.EventMesh.Protocols.AMQP.Framing
{
    public struct ProtocolHeader
    {
        public static ProtocolHeader SASLHeaderNegotiation = new ProtocolHeader
        {
            Id = 3,
            Major = 1
        };
        public static ProtocolHeader SASLHeaderSecuredConnection = new ProtocolHeader
        {
            Id = 0,
            Major = 1
        };
        public byte Id;

        public byte Major;

        public byte Minor;

        public byte Revision;

        public static ProtocolHeader Create(byte[] buffer, int offset)
        {
            if (buffer[offset + 0] != (byte)'A' ||
                buffer[offset + 1] != (byte)'M' ||
                buffer[offset + 2] != (byte)'Q' ||
                buffer[offset + 3] != (byte)'P')
            {
                throw new InvalidOperationException("ProtocolName Expect:AMQP Actual:" + new string(Encoding.UTF8.GetChars(buffer, offset, 4)));
            }

            return new ProtocolHeader()
            {
                Id = buffer[offset + 4],
                Major = buffer[offset + 5],
                Minor = buffer[offset + 6],
                Revision = buffer[offset + 7]
            };
        }

        public byte[] Serialize()
        {
            return new byte[FixedWidth.ULong]
            {
                (byte)'A',
                (byte)'M',
                (byte)'Q',
                (byte)'P',
                Id,
                Major,
                Minor,
                Revision
            };
        }
    }
}
