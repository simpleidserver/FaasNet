using Amqp;
using Amqp.Types;

namespace FaasNet.EventMesh.Protocols.AMQP.Extensions
{
    public static class MessageExtensions
    {
        public static ByteBuffer Serialize(this Message message)
        {
            const int reservedBytes = 40;
            var messageByteBuffer = new ByteBuffer(reservedBytes + message.GetEstimatedMessageSize(), false);
            messageByteBuffer.AdjustPosition(messageByteBuffer.Offset + reservedBytes, 0);
            EncodeIfNotNull(message.Header, messageByteBuffer);
            EncodeIfNotNull(message.DeliveryAnnotations, messageByteBuffer);
            EncodeIfNotNull(message.MessageAnnotations, messageByteBuffer);
            EncodeIfNotNull(message.Properties, messageByteBuffer);
            EncodeIfNotNull(message.ApplicationProperties, messageByteBuffer);
            EncodeIfNotNull(message.BodySection, messageByteBuffer);
            EncodeIfNotNull(message.Footer, messageByteBuffer);
            return messageByteBuffer;
        }

        private static void EncodeIfNotNull(RestrictedDescribed section, ByteBuffer buffer)
        {
            if (section != null)
            {
                section.Encode(buffer);
            }
        }
    }
}
