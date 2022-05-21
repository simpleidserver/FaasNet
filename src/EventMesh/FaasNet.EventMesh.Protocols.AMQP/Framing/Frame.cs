using Amqp;
using Amqp.Framing;
using Amqp.Types;
using System;

namespace FaasNet.EventMesh.Protocols.AMQP.Framing
{
    public enum FrameTypes : byte
    {
        /// <summary>
        /// AMQP Frame.
        /// </summary>
        Amqp = 0,
        /// <summary>
        /// SASL Frame.
        /// </summary>
        Sasl = 1
    }

    public class Frame
    {
        public const int CmdBufferSize = 128;

        /// <summary>
        /// Byte 4 of the frame header is the data offset.
        /// This gives the position of the body within the frame.
        /// </summary>
        private const byte DOFF = 2;
        
        public FrameTypes Type { get; set; }
        public ushort Channel { get; set; }

        public static void Decode(ByteBuffer buffer, out ushort channel, out DescribedList command)
        {
            AmqpBitConverter.ReadUInt(buffer);
            AmqpBitConverter.ReadUByte(buffer);
            AmqpBitConverter.ReadUByte(buffer);
            channel = AmqpBitConverter.ReadUShort(buffer);
            if (buffer.Length > 0)
            {
                var tt = Encoder.ReadDescribed(buffer, Encoder.ReadFormatCode(buffer));
                command = (DescribedList)tt;
            }
            else
            {
                command = null;
            }
        }

        public ByteBuffer Serialize(DescribedList cmd)
        {
            var buffer = new ByteBuffer(CmdBufferSize, false);
            buffer.Append(FixedWidth.UInt);
            AmqpBitConverter.WriteUByte(buffer, DOFF);
            AmqpBitConverter.WriteUByte(buffer, (byte)Type);
            AmqpBitConverter.WriteUShort(buffer, Channel);
            cmd.Encode(buffer);
            AmqpBitConverter.WriteInt(buffer.Buffer, buffer.Offset, buffer.Length);
            return buffer;
        }
    }
}
