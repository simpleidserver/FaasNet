using CloudNative.CloudEvents;
using EventMesh.Runtime.Exceptions;
using EventMesh.Runtime.MessageBroker;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
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
        public async Task When_SendHeartBeat_Then_SuccessfulResponseIsReturned()
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

        #region Hello Request

        [Fact]
        public async Task When_SendHello_Then_SessionIsCreated()
        {
            // ARRANGE
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4990;
            });
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient(port: 4990);
            var response = await client.Hello(new UserAgent
            {
                ClientId = "clientId",
                Environment = "TST",
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

        #endregion

        #region Disconnect

        [Fact]
        public async Task When_Disconnect_And_ThereIsNoActiveSession_Then_ErrorIsReturned()
        {
            // ARRANGE
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4994;
            });
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient(port: 4994);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.Disconnect("clientid", "sessionid"));
            host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.INVALID_CLIENT, exception.Error);
        }

        [Fact]
        public async Task When_Disconnect_Then_SessionIsNotActive()
        {
            // ARRANGE
            const string clientId = "clientId";
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4995;
            });
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient(port: 4995);
            var helloResponse = await client.Hello(new UserAgent
            {
                ClientId = clientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0"
            });
            var disconnectResponse = await client.Disconnect(clientId, helloResponse.SessionId);
            host.Stop();

            // ASSERT
            Assert.Equal(Commands.DISCONNECT_RESPONSE, disconnectResponse.Header.Command);
            Assert.Equal(HeaderStatus.SUCCESS, disconnectResponse.Header.Status);
        }

        #endregion

        #region Add Bridge

        [Fact]
        public async Task When_Add_NotReachableBridge_Then_ErrorIsReturned()
        {
            // ARRANGE
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4996;
            });
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient(port: 4996);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.AddBridge("urn", 4997, CancellationToken.None));
            host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.INVALID_BRIDGE, exception.Error);
        }

        [Fact]
        public async Task When_AddSameBridgeTwice_Then_ErrorIsReturned()
        {
            // ARRANGE
            var firstBuilder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4997;
                opt.Urn = "localhost";
            });
            var secondBuilder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4998;
                opt.Urn = "localhost";
            });
            var firstHost = firstBuilder.Build();
            firstHost.Run();
            var secondHost = secondBuilder.Build();
            secondHost.Run();

            // ACT
            var client = new RuntimeClient(port: 4997);
            await client.AddBridge("localhost", 4998, CancellationToken.None);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.AddBridge("localhost", 4998, CancellationToken.None));
            firstHost.Stop();
            secondHost.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.BRIDGE_EXISTS, exception.Error);
        }

        #endregion

        #region Subscribe

        [Fact]
        public async Task When_Subscribe_And_CloseUdpClient_Then_SessionIsRemoved()
        {
            // ARRANGE
            const string clientId = "clientId";
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4991;
            });
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var clientStore = serviceProvider.GetRequiredService<IClientStore>();
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient(port: 4991);
            var response = await client.Hello(new UserAgent
            {
                ClientId = clientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0"
            });
            client.UdpClient.Close();
            var exception = await Assert.ThrowsAsync<RuntimeClientSessionClosedException>(async () => await client.Subscribe(clientId, response.SessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = "topic"
                }
            }));
            host.Stop();

            // ASSERT
            var cl = clientStore.Get(clientId);
            var lastSession = cl.Sessions.First();
            Assert.False(cl.ActiveSessions.Any());
            Assert.True(cl.Sessions.Count() == 1);
            Assert.Equal(ClientSessionState.FINISH, lastSession.State);
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task When_Subscribe_And_ThereIsNoActiveSession_Then_ExceptionIsThrown()
        {
            // ARRANGE
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4992;
            });
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient(port: 4992);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.Subscribe("clientId", "sessionId", new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = "topic"
                }
            }));
            host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.INVALID_CLIENT, exception.Error);
        }

        [Fact]
        public async Task When_Subscribe_And_SessionDoesntHaveCorrectPurpose_ThenExceptionIsThrown()
        {
            // ARRANGE
            const string topicName = "Test.COUCOU";
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
            var topics = new ConcurrentBag<InMemoryTopic>
            {
                new InMemoryTopic
                {
                    TopicName = topicName
                }
            };
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5003;
            });
            builder.AddInMemoryMessageBroker(topics);
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var messagePublisher = serviceProvider.GetRequiredService<IMessagePublisher>();
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient(port: 5003);
            var helloResponse = await client.Hello(new UserAgent
            {
                ClientId = "id",
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.PUB,
                Version = "0",
                BufferCloudEvents = 1
            });
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.Subscribe("id", helloResponse.SessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = topicName
                }
            }));
            host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.UNAUTHORIZED_SUBSCRIBE, exception.Error);
        }

        [Fact]
        public async Task When_Subscribe_ToOneTopic_Then_MessagesAreReceived()
        {
            // ARRANGE
            const string topicName = "Test.COUCOU";
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
            var topics = new ConcurrentBag<InMemoryTopic>
            {
                new InMemoryTopic
                {
                    TopicName = topicName
                }
            };
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4993;
            });
            builder.AddInMemoryMessageBroker(topics);
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var messagePublisher = serviceProvider.GetRequiredService<IMessagePublisher>();
            var clientStore = serviceProvider.GetRequiredService<IClientStore>();
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient(port: 4993);
            var helloResponse = await client.Hello(new UserAgent
            {
                ClientId = "id",
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 1
            });
            await client.Subscribe("id", helloResponse.SessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = topicName
                }
            }, (m) =>
            {
                msg = m;
            });
            await messagePublisher.Publish(cloudEvent, topicName, Client.Create("clientId", "urn"));
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

        [Fact]
        public async Task When_AddBridge_And_Subscribe_ToOneTopic_Then_MessageIsReceivedFromSecondServer()
        {
            // ARRANGE
            const string topicName = "Test.COUCOU";
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
            var topics = new ConcurrentBag<InMemoryTopic>
            {
                new InMemoryTopic
                {
                    TopicName = topicName
                }
            };
            var firstBuilder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5001;
                opt.Urn = "localhost";
            });
            var secondBuilder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5002;
                opt.Urn = "localhost";
            }).AddInMemoryMessageBroker(topics);
            var firstServiceProvider = firstBuilder.ServiceCollection.BuildServiceProvider();
            var secondServiceProvider = secondBuilder.ServiceCollection.BuildServiceProvider();
            var messagePublisher = secondServiceProvider.GetRequiredService<IMessagePublisher>();
            var firstClientStore = firstServiceProvider.GetRequiredService<IClientStore>();
            var secondClientStore = secondServiceProvider.GetRequiredService<IClientStore>();
            var firstHost = firstBuilder.Build();
            firstHost.Run();
            var secondHost = secondBuilder.Build();
            secondHost.Run();

            // ACT
            var client = new RuntimeClient(port: 5001);
            await client.AddBridge("localhost", 5002, CancellationToken.None);
            var helloResponse = await client.Hello(new UserAgent
            {
                ClientId = "id",
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 1
            });
            var subscriptionResult = await client.Subscribe("id", helloResponse.SessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = topicName
                }
            }, (m) =>
            {
                msg = m;
            });
            await messagePublisher.Publish(cloudEvent, topicName, Client.Create("clientId", "urn"));
            while (msg == null)
            {
                Thread.Sleep(100);
            }
            subscriptionResult.Stop();

            await client.Disconnect("id", helloResponse.SessionId);
            firstHost.Stop();
            secondHost.Stop();

            // ASSERT
            var firstServerClient = firstClientStore.Get("id");
            var secondServerClient = secondClientStore.Get("id");
            var secondTopic = secondServerClient.Topics.First();
            var firstSession = firstServerClient.Sessions.First();
            var secondSession = secondServerClient.Sessions.First();
            Assert.Equal(ClientSessionState.FINISH, firstSession.State);
            Assert.Equal(ClientSessionState.FINISH, secondSession.State);
            Assert.Equal(1, secondTopic.Offset);
            Assert.Equal("inmemory", secondTopic.BrokerName);
            Assert.Equal("Test.COUCOU", secondTopic.Name);
        }

        #endregion

        #region Publish Message

        [Fact]
        public async Task When_PublishMessage_And_ThereIsNoActiveSession_Then_ExceptionIsThrown()
        {
            // ARRANGE
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
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5004;
            });
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient(port: 5004);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.PublishMessage("clientId", "sessionId", "topic", cloudEvent));
            host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.INVALID_CLIENT, exception.Error);
        }

        [Fact]
        public async Task When_PublishMessage_And_SessionDoesntHaveCorrectPurpose_ThenExceptionIsThrown()
        {
            // ARRANGE
            const string topicName = "Test.COUCOU";
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
            var topics = new ConcurrentBag<InMemoryTopic>
            {
                new InMemoryTopic
                {
                    TopicName = topicName
                }
            };
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5005;
            });
            builder.AddInMemoryMessageBroker(topics);
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var messagePublisher = serviceProvider.GetRequiredService<IMessagePublisher>();
            var host = builder.Build();
            host.Run();

            // ACT
            var client = new RuntimeClient(port: 5005);
            var helloResponse = await client.Hello(new UserAgent
            {
                ClientId = "id",
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 1
            });
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.PublishMessage("id", helloResponse.SessionId, "topic", cloudEvent));
            host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.UNAUTHORIZED_PUBLISH, exception.Error);
        }

        [Fact]
        public async Task When_PublishMessage_And_SubscribeToOneTopic_Then_MessageIsReceived()
        {
            // ARRANGE
            const string topicName = "Test.COUCOU";
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
            var topics = new ConcurrentBag<InMemoryTopic>
            {
                new InMemoryTopic
                {
                    TopicName = topicName
                }
            };
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5006;
            });
            builder.AddInMemoryMessageBroker(topics);
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var clientStore = serviceProvider.GetRequiredService<IClientStore>();
            var host = builder.Build();
            host.Run();

            // ACT
            var subClient = new RuntimeClient(port: 5006);
            var subClientHelloResponse = await subClient.Hello(new UserAgent
            {
                ClientId = "subClientId",
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 1
            });
            await subClient.Subscribe("subClientId", subClientHelloResponse.SessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = topicName
                }
            }, (m) =>
            {
                msg = m;
            });
            var pubClient = new RuntimeClient(port: 5006);
            var pubClientHelloResponse = await pubClient.Hello(new UserAgent
            {
                ClientId = "pubClientId",
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.PUB,
                Version = "0",
                BufferCloudEvents = 1
            });
            await pubClient.PublishMessage("pubClientId", pubClientHelloResponse.SessionId, topicName, cloudEvent);
            while (msg == null)
            {
                Thread.Sleep(100);
            }

            host.Stop();

            // ASSERT
            var storedClient = clientStore.Get("subClientId");
            var topic = storedClient.Topics.First();
            Assert.NotNull(msg);
            Assert.Equal(Constants.InMemoryBrokername, topic.BrokerName);
            Assert.Equal(1, topic.Offset);
            Assert.Equal(topicName, topic.Name);
        }

        [Fact]
        public async Task When_Publishmessage_And_Subscribe_ToOneTopic_Then_MessageIsReceivedFromSecondServer()
        {
            // ARRANGE
            const string topicName = "Test.COUCOU";
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
            var topics = new ConcurrentBag<InMemoryTopic>
            {
                new InMemoryTopic
                {
                    TopicName = topicName
                }
            };
            var firstBuilder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5007;
                opt.Urn = "localhost";
            });
            var secondBuilder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5008;
                opt.Urn = "localhost";
            }).AddInMemoryMessageBroker(topics);
            var firstServiceProvider = firstBuilder.ServiceCollection.BuildServiceProvider();
            var secondServiceProvider = secondBuilder.ServiceCollection.BuildServiceProvider();
            var firstClientStore = firstServiceProvider.GetRequiredService<IClientStore>();
            var secondClientStore = secondServiceProvider.GetRequiredService<IClientStore>();
            var firstHost = firstBuilder.Build();
            firstHost.Run();
            var secondHost = secondBuilder.Build();
            secondHost.Run();

            // ACT
            var addBridgeClient = new RuntimeClient(port: 5007);
            await addBridgeClient.AddBridge("localhost", 5008, CancellationToken.None);
            var subClient = new RuntimeClient(port: 5007);
            var subClientHelloResponse = await subClient.Hello(new UserAgent
            {
                ClientId = "subClientId",
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 1
            });
            var subscriptionResult = await subClient.Subscribe("subClientId", subClientHelloResponse.SessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = topicName
                }
            }, (m) =>
            {
                msg = m;
            });
            var pubClient = new RuntimeClient(port: 5007);
            var pubClientHelloResponse = await pubClient.Hello(new UserAgent
            {
                ClientId = "pubClientId",
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.PUB,
                Version = "0",
                BufferCloudEvents = 1
            });
            await pubClient.PublishMessage("pubClientId", pubClientHelloResponse.SessionId, topicName, cloudEvent);
            while (msg == null)
            {
                Thread.Sleep(100);
            }

            subscriptionResult.Stop();

            await subClient.Disconnect("subClientId", subClientHelloResponse.SessionId);
            firstHost.Stop();
            secondHost.Stop();

            // ASSERT
            var firstServerClient = firstClientStore.Get("subClientId");
            var secondServerClient = secondClientStore.Get("subClientId");
            var secondTopic = secondServerClient.Topics.First();
            var firstSession = firstServerClient.Sessions.First();
            var secondSession = secondServerClient.Sessions.First();
            Assert.Equal(ClientSessionState.FINISH, firstSession.State);
            Assert.Equal(ClientSessionState.FINISH, secondSession.State);
            Assert.Equal(1, secondTopic.Offset);
            Assert.Equal("inmemory", secondTopic.BrokerName);
            Assert.Equal("Test.COUCOU", secondTopic.Name);
        }

        #endregion
    }
}
