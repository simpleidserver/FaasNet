using EventMesh.Runtime.Messages;
using Xunit;

namespace EventMesh.Runtime.Tests
{
    public class EventMeshMessageSerializerFixture
    {
        [Fact]
        public void Can_Serialize_And_Deserialize_Heartbeat()
        {
            // ARRANGE
            var message = EventMeshMessageRequestBuilder.HeartBeat();
            var ctx = new EventMeshWriterBufferContext();
            message.Serialize(ctx);

            // ACT
            var newMessage = EventMeshPackage.Deserialize(new EventMeshReaderBufferContext(ctx.Buffer.ToArray()));

            // ASSERT
            Assert.Equal(message.Header.Command.Code, newMessage.Header.Command.Code);
            Assert.Equal(message.Header.Seq, newMessage.Header.Seq);
            Assert.Equal(message.Header.Status.Code, newMessage.Header.Status.Code);
            Assert.Equal(message.Header.Status.Desc, newMessage.Header.Status.Desc);
        }
    }
}
