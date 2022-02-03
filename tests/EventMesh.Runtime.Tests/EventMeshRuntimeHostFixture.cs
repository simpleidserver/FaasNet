using EventMesh.Runtime.Messages;
using System.Threading.Tasks;
using Xunit;

namespace EventMesh.Runtime.Tests
{
    public class EventMeshRuntimeHostFixture
    {
        [Fact]
        public async Task Send_HeartBeat()
        {
            // ARRANGE
            var builder = new RuntimeHostBuilder();
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient();
            var response = await client.HeartBeat();
            host.Stop();

            // ASSERT
            Assert.Equal(Commands.HEARTBEAT_RESPONSE, response.Header.Command);
            Assert.Equal(HeaderStatus.SUCCESS.Code, response.Header.Status.Code);
            Assert.Equal(HeaderStatus.SUCCESS.Desc, response.Header.Status.Desc);
        }

        [Fact]
        public async Task Send_Hello()
        {
            // ARRANGE
            var builder = new RuntimeHostBuilder();
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient();
            var response = await client.Hello(new UserAgent
            {
                Environment = "TST",
                Username = "userName",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0"
            });
            host.Stop();

            // ASSERT
            Assert.Equal(Commands.HELLO_RESPONSE, response.Header.Command);
            Assert.Equal(HeaderStatus.SUCCESS.Code, response.Header.Status.Code);
            Assert.Equal(HeaderStatus.SUCCESS.Desc, response.Header.Status.Desc);
        }
    }
}
