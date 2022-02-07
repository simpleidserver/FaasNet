using CloudNative.CloudEvents;
using EventMesh.Runtime.MessageBroker;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        [Fact]
        public async Task Send_Subscribe()
        {
            const string topicName = "Test.COUCOU";
            // ARRANGE
            AsyncMessageToClient msg = null;
            var cloudEvent = new CloudEvent
            {
                Type = "com.github.pull.create",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = "testttt",
                ["comexampleextension1"] = "value"
            };
            var topics = new List<InMemoryTopic>
            {
                new InMemoryTopic
                {
                    TopicName = topicName
                }
            };
            var builder = new RuntimeHostBuilder();
            builder.AddInMemoryMessageBroker(topics);
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var messagePublisher = serviceProvider.GetRequiredService<IMessagePublisher>();
            var clientStore = serviceProvider.GetRequiredService<IClientStore>();
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient();
            await client.Hello(new UserAgent
            {
                ClientId = "id",
                Environment = "TST",
                Username = "userName",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 1
            });
            await client.Subscribe(new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = topicName
                }
            }, (m) =>
            {
                msg = m;
            });
            await messagePublisher.Publish(cloudEvent, topicName);
            while (msg == null)
            {
                Thread.Sleep(100);
            }

            // ASSERT
            var storedClient = clientStore.Get("id");
            var topic = storedClient.Topics.First();
            Assert.NotNull(msg);
            Assert.Equal(Constants.InMemoryBrokername, topic.BrokerName);
            Assert.Equal(1, topic.Offset);
            Assert.Equal(topicName, topic.Name);
        }
    }
}
