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
            var builder = new EventMeshRuntimeHostBuilder();
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new EventMeshRuntimeClient();
            var response = await client.HeartBeat();
            host.Stop();

            // ASSERT
            Assert.Equal(EventMeshCommands.HEARTBEAT_RESPONSE, response.Header.Command);
            Assert.Equal(EventMeshHeaderStatus.SUCCESS.Code, response.Header.Status.Code);
            Assert.Equal(EventMeshHeaderStatus.SUCCESS.Desc, response.Header.Status.Desc);
        }
    }
}
