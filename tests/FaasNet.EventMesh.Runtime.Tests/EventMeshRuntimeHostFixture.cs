using CloudNative.CloudEvents;
using FaasNet.Common;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Exceptions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FaasNet.EventMesh.Runtime.Tests
{
    public class EventMeshRuntimeHostFixture
    {
        [Fact]
        public async Task When_Ping_Then_NoExceptionIsThrown()
        {
            // ARRANGE
            var host = new ServiceCollection().AddEventMeshServer().Services.BuildServiceProvider().GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            await client.Ping();
            await host.Stop();
        }

        #region Ge all VPNS

        [Fact]
        public async Task When_GetAllVpns_Then_VpnsAreReturned()
        {
            // ARRANGE
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                Vpn.Create("default", "default")
            }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            var response = await client.GetAllVpns();
            await host.Stop();

            // ASSERT
            Assert.Single(response);
            Assert.Equal("default", response.First());
        }

        #endregion

        #region Create session

        [Fact]
        public async Task When_CreateSession_And_VpnDoesntExist_Then_ErrorIsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.SUB });
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.CreateSubSession("wrongVpn", "clientId"));
            await host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.UNKNOWN_VPN, exception.Error);
        }

        [Fact]
        public async Task When_CreateSession_And_ClientDoesntExist_Then_Error_IsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.SUB });
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.CreateSubSession(vpn.Name, "wrongClientId"));
            await host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.INVALID_CLIENT, exception.Error);
        }

        [Fact]
        public async Task When_CreateSession_And_PurposeIsNotCorrect_Then_Error_IsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.SUB });
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.CreatePubSession(vpn.Name, newClient.ClientId, null, CancellationToken.None));
            await host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.NOT_AUTHORIZED, exception.Error);
        }

        [Fact]
        public async Task When_CreatePubSession_And_SessionLifeTimeIsTooLong_Then_Error_IsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.CreatePubSession(vpn.Name, newClient.ClientId, TimeSpan.FromHours(5), CancellationToken.None));
            await host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.SESSION_LIFETIME_TOOLONG, exception.Error);
        }

        [Fact]
        public async Task When_CreatePubSession_And_SessionIsInfinite_Then_Error_IsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.CreatePubSession(vpn.Name, newClient.ClientId, null, true, CancellationToken.None));
            await host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.SESSION_LIFETIME_CANNOT_BE_INFINITE, exception.Error);
        }

        [Fact]
        public async Task When_CreateSubSession_And_SessionLifeTimeIsTooShort_Then_Error_IsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.SUB });
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.CreateSubSession(vpn.Name, newClient.ClientId, TimeSpan.FromSeconds(10), CancellationToken.None));
            await host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.SESSION_LIFETIME_TOOSHORT, exception.Error);
        }

        [Fact]
        public async Task When_Create_SubSession_Then_NoError_IsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.SUB });
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            await client.CreateSubSession(vpn.Name, newClient.ClientId, null, CancellationToken.None);
            await host.Stop();
        }

        [Fact]
        public async Task When_Create_PubSession_Then_NoError_IsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            await client.CreatePubSession(vpn.Name, newClient.ClientId, null, CancellationToken.None);
            await host.Stop();
        }

        #endregion

        #region Disconnect

        [Fact]
        public async Task When_Disconnect_And_ThereIsNoActiveSession_Then_ErrorIsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.Disconnect(newClient.ClientId, "sessionid"));
            await host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.INVALID_SESSION, exception.Error);
        }

        [Fact]
        public async Task When_Disconnect_PubSession_Then_SessionIsNotActive()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
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
            var client = new EventMeshClient();
            var pubSession = await client.CreatePubSession(vpn.Name, newClient.ClientId, null, CancellationToken.None);
            await pubSession.Disconnect(CancellationToken.None);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await pubSession.Publish("topic", cloudEvent, CancellationToken.None));
            await host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.NO_ACTIVE_SESSION, exception.Error);
        }

        [Fact]
        public async Task When_Disconnect_SubSession_Then_SessionIsNotActive()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.SUB });
            var serviceProvider = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider;
            var host = serviceProvider.GetRequiredService<INodeHost>();
            var clientSessionStore = serviceProvider.GetRequiredService<IClientSessionStore>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            var pubSession = await client.CreateSubSession(vpn.Name, newClient.ClientId, null, CancellationToken.None);
            pubSession.DirectSubscribe("topic", (c) => { }, CancellationToken.None);
            await pubSession.Disconnect(CancellationToken.None);
            await host.Stop();

            // ASSERT
            var clientSession = await clientSessionStore.Get(pubSession.SessionId, CancellationToken.None);
            Assert.NotNull(clientSession);
            Assert.Equal(ClientSessionState.FINISH, clientSession.State);
        }

        #endregion

        #region Add bridge

        [Fact]
        public async Task When_AddNotReachableBridge_Then_ErrorIsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
            var host = new ServiceCollection().AddEventMeshServer().AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient();
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.AddBridge(vpn.Name, "urn", 6000, vpn.Name, CancellationToken.None));
            await host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.TARGET_NOT_REACHABLE, exception.Error);
        }

        #endregion

        /*

        #region Add Bridge

        [Fact]
        public async Task When_AddSameBridgeTwice_Then_ErrorIsReturned()
        {
            // ARRANGE
            var firstVpn = Vpn.Create("default", "default");
            var secondVpn = Vpn.Create("default", "default");
            var firstVpnClient = Models.Client.Create(firstVpn.Name, "clientId", "urn");
            var secondVpnClient = Models.Client.Create(secondVpn.Name, "clientId", "urn");
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
            var firstHost = firstBuilder.AddVpns(new List<Vpn> { firstVpn }).AddClients(new List<Models.Client> { firstVpnClient }).Build();
            await firstHost.Run(CancellationToken.None);
            var secondHost = secondBuilder.AddVpns(new List<Vpn> { secondVpn }).AddClients(new List<Models.Client> { secondVpnClient }).Build();
            await secondHost.Run(CancellationToken.None);

            // ACT
            var client = new RuntimeClient(port: 4997);
            await client.AddBridge(firstVpn.Name, "localhost", 4998, secondVpn.Name, CancellationToken.None);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.AddBridge(firstVpn.Name, "localhost", 4998, secondVpn.Name, CancellationToken.None));
            await firstHost.Stop(CancellationToken.None);
            await secondHost.Stop(CancellationToken.None);

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
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn");
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4991;
            });
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var host = builder.AddVpns(new List<Vpn> { vpn }).AddClients(new List<Models.Client> { newClient }).Build();
            await host.Run(CancellationToken.None);

            // ACT
            var client = new RuntimeClient(port: 4991);
            var response = await client.Hello(new UserAgent
            {
                ClientId = newClient.ClientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                Vpn = vpn.Name
            });
            client.UdpClient.Close();
            var exception = await Assert.ThrowsAsync<RuntimeClientSessionClosedException>(async () => await client.Subscribe(newClient.ClientId, response.SessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = "topic"
                }
            }));
            await host.Stop(CancellationToken.None);

            // ASSERT
            var lastSession = newClient.Sessions.First();
            Assert.False(newClient.ActiveSessions.Any());
            Assert.True(newClient.Sessions.Count() == 1);
            Assert.Equal(ClientSessionState.FINISH, lastSession.State);
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task When_Subscribe_And_ThereIsNoActiveSession_Then_ExceptionIsThrown()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn");
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4992;
            });
            var host = builder.AddVpns(new List<Vpn> { vpn }).AddClients(new List<Models.Client> { newClient }).Build();
            await host.Run(CancellationToken.None);

            // ACT
            var client = new RuntimeClient(port: 4992);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.Subscribe(newClient.ClientId, "sessionId", new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = "topic"
                }
            }));
            await host.Stop(CancellationToken.None);

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.INVALID_SESSION, exception.Error);
        }

        [Fact]
        public async Task When_Subscribe_And_SessionDoesntHaveCorrectPurpose_ThenExceptionIsThrown()
        {
            // ARRANGE
            const string topicName = "Test.COUCOU";
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
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
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5003;
            });
            builder.AddInMemoryMessageBroker();
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var messagePublisher = serviceProvider.GetRequiredService<IMessagePublisher>();
            var host = builder.AddVpns(new List<Vpn> { vpn }).AddClients(new List<Models.Client> { newClient }).Build();
            await host.Run(CancellationToken.None);

            // ACT
            var client = new RuntimeClient(port: 5003);
            var helloResponse = await client.Hello(new UserAgent
            {
                ClientId = newClient.ClientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.PUB,
                Version = "0",
                BufferCloudEvents = 1,
                Vpn = vpn.Name
            });
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.Subscribe(newClient.ClientId, helloResponse.SessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = topicName
                }
            }));
            await host.Stop(CancellationToken.None);

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
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.SUB });
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
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4993;
            });
            builder.AddInMemoryMessageBroker();
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var messagePublisher = serviceProvider.GetRequiredService<IMessagePublisher>();
            var vpnStore = serviceProvider.GetRequiredService<IVpnStore>();
            var host = builder.AddVpns(new List<Vpn> { vpn }).AddClients(new List<Models.Client> { newClient }).Build();
            await host.Run(CancellationToken.None);

            // ACT
            var client = new RuntimeClient(port: 4993);
            var helloResponse = await client.Hello(new UserAgent
            {
                ClientId = newClient.ClientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 1,
                Vpn = vpn.Name
            });
            await client.Subscribe(newClient.ClientId, helloResponse.SessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = topicName
                }
            }, (m) =>
            {
                msg = m;
            });
            await messagePublisher.Publish(cloudEvent, topicName, newClient);
            while (msg == null)
            {
                Thread.Sleep(100);
            }

            // ASSERT
            var topic = newClient.Topics.First();
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
            var firstVpn = Vpn.Create("default", "default");
            var secondVpn = Vpn.Create("default", "default");
            var firstVpnClient = Models.Client.Create(firstVpn.Name, "clientId", "urn");
            var secondVpnClient = Models.Client.Create(secondVpn.Name, "clientId", "urn");
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
            var firstBuilder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5001;
                opt.Urn = "localhost";
            }).AddInMemoryMessageBroker();
            var secondBuilder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5002;
                opt.Urn = "localhost";
            }).AddInMemoryMessageBroker();
            var secondServiceProvider = secondBuilder.ServiceCollection.BuildServiceProvider();
            var messagePublisher = secondServiceProvider.GetRequiredService<IMessagePublisher>();
            var firstHost = firstBuilder.AddVpns(new List<Vpn> { firstVpn }).AddClients(new List<Models.Client> { firstVpnClient }).Build();
            await firstHost.Run(CancellationToken.None);
            var secondHost = secondBuilder.AddVpns(new List<Vpn> { secondVpn }).AddClients(new List<Models.Client> { secondVpnClient }).Build();
            await secondHost.Run(CancellationToken.None);

            // ACT
            var client = new RuntimeClient(port: 5001);
            await client.AddBridge(firstVpn.Name, "localhost", 5002, secondVpn.Name, CancellationToken.None);
            var helloResponse = await client.Hello(new UserAgent
            {
                ClientId = firstVpnClient.ClientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 1,
                Vpn = firstVpn.Name
            });
            var subscriptionResult = await client.Subscribe(firstVpnClient.ClientId, helloResponse.SessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = topicName
                }
            }, (m) =>
            {
                msg = m;
            });
            await messagePublisher.Publish(cloudEvent, topicName, secondVpnClient);
            while (msg == null)
            {
                Thread.Sleep(100);
            }
            await subscriptionResult.Stop(CancellationToken.None);
            var firstServerClient = firstVpnClient;
            var secondServerClient = secondVpnClient;
            var secondTopic = secondServerClient.Topics.First();
            var firstSession = firstServerClient.Sessions.First();
            var secondSession = secondServerClient.Sessions.First();
            while (firstSession.State != ClientSessionState.FINISH)
            {
                Thread.Sleep(100);
            }

            await firstHost.Stop(CancellationToken.None);
            await secondHost.Stop(CancellationToken.None);

            // ASSERT
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
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn");
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
            var host = builder.AddVpns(new List<Vpn> { vpn }).AddClients(new List<Models.Client> { newClient }).Build();
            await host.Run(CancellationToken.None);

            // ACT
            var client = new RuntimeClient(port: 5004);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.PublishMessage(newClient.ClientId, "sessionId", "topic", cloudEvent));
            await host.Stop(CancellationToken.None);

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.INVALID_SESSION, exception.Error);
        }

        [Fact]
        public async Task When_PublishMessage_And_SessionDoesntHaveCorrectPurpose_ThenExceptionIsThrown()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.SUB });
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
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5005;
            });
            builder.AddInMemoryMessageBroker();
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var messagePublisher = serviceProvider.GetRequiredService<IMessagePublisher>();
            var host = builder.AddVpns(new List<Vpn> { vpn }).AddClients(new List<Models.Client> { newClient }).Build();
            await host.Run(CancellationToken.None);

            // ACT
            var client = new RuntimeClient(port: 5005);
            var helloResponse = await client.Hello(new UserAgent
            {
                ClientId = newClient.ClientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 1,
                Vpn = vpn.Name
            });
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.PublishMessage(newClient.ClientId, helloResponse.SessionId, "topic", cloudEvent));
            await host.Stop(CancellationToken.None);

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
            var vpn = Vpn.Create("default", "default");
            var firstSubClient = Models.Client.Create(vpn.Name, "subClientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.SUB });
            var firstPubClient = Models.Client.Create(vpn.Name, "pubClientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
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
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 5006;
            });
            builder.AddInMemoryMessageBroker();
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var host = builder.AddVpns(new List<Vpn> { vpn }).AddClients(new List<Models.Client> { firstSubClient, firstPubClient }).Build();
            await host.Run(CancellationToken.None);

            // ACT
            var subClient = new RuntimeClient(port: 5006);
            var subClientHelloResponse = await subClient.Hello(new UserAgent
            {
                ClientId = firstSubClient.ClientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 1,
                Vpn = vpn.Name
            });
            await subClient.Subscribe(firstSubClient.ClientId, subClientHelloResponse.SessionId, new List<SubscriptionItem>
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
                ClientId = firstPubClient.ClientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.PUB,
                Version = "0",
                BufferCloudEvents = 1,
                Vpn = vpn.Name
            });
            await pubClient.PublishMessage(firstPubClient.ClientId, pubClientHelloResponse.SessionId, topicName, cloudEvent);
            while (msg == null)
            {
                Thread.Sleep(100);
            }

            await host.Stop(CancellationToken.None);

            // ASSERT
            var topic = firstSubClient.Topics.First();
            Assert.NotNull(msg);
            Assert.Equal(Constants.InMemoryBrokername, topic.BrokerName);
            Assert.Equal(1, topic.Offset);
            Assert.Equal(topicName, topic.Name);
        }

        #endregion
        */
    }
}
