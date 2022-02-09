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
            var message = PackageRequestBuilder.HeartBeat();
            var ctx = new WriteBufferContext();
            message.Serialize(ctx);

            // ACT
            var newMessage = Package.Deserialize(new ReadBufferContext(ctx.Buffer.ToArray()));

            // ASSERT
            Assert.Equal(message.Header.Command.Code, newMessage.Header.Command.Code);
            Assert.Equal(message.Header.Seq, newMessage.Header.Seq);
            Assert.Equal(message.Header.Status.Code, newMessage.Header.Status.Code);
            Assert.Equal(message.Header.Status.Desc, newMessage.Header.Status.Desc);
        }

        [Fact]
        public void Can_Serialize_And_Deserialize_HelloRequest()
        {
            // ARRANGE
            var message = PackageRequestBuilder.Hello(new UserAgent
            {
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0"
            });
            var ctx = new WriteBufferContext();
            message.Serialize(ctx);

            // ACT
            var newMessage = Package.Deserialize(new ReadBufferContext(ctx.Buffer.ToArray())) as HelloRequest;

            // ASSERT
            Assert.Equal(message.Header.Command.Code, newMessage.Header.Command.Code);
            Assert.Equal(message.Header.Seq, newMessage.Header.Seq);
            Assert.Equal(message.Header.Status.Code, newMessage.Header.Status.Code);
            Assert.Equal(message.Header.Status.Desc, newMessage.Header.Status.Desc);
            Assert.Equal(message.UserAgent.Environment, newMessage.UserAgent.Environment);
            Assert.Equal(message.UserAgent.Password, newMessage.UserAgent.Password);
            Assert.Equal(message.UserAgent.Pid, newMessage.UserAgent.Pid);
            Assert.Equal(message.UserAgent.Purpose.Code, newMessage.UserAgent.Purpose.Code);
            Assert.Equal(message.UserAgent.Version, newMessage.UserAgent.Version);
        }
    }
}
