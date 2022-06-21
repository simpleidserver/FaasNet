using CloudNative.CloudEvents;
using FaasNet.Common;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Exceptions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.EventMesh.Sink;
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
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4000).Services.BuildServiceProvider().GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4000);
            await client.Ping();
            await host.Stop();
        }

        #region Get all VPNS

        [Fact]
        public async Task When_GetAllVpns_Then_VpnsAreReturned()
        {
            // ARRANGE
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4001).AddVpns(new List<Vpn>
            {
                Vpn.Create("default", "default")
            }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4001);
            var response = await client.GetAllVpns();
            await host.Stop();

            // ASSERT
            Assert.Single(response);
            Assert.Equal("default", response.First());
        }

        #endregion

        #region Create session

        // [Fact]
        public async Task When_CreateSession_And_VpnDoesntExist_Then_ErrorIsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.SUB });
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4002).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4002);
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
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4003).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4003);
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
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4004).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4004);
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
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4005).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4005);
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
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4006).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4006);
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
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4007).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4007);
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
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4008).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4008);
            await client.CreateSubSession(vpn.Name, newClient.ClientId, null, CancellationToken.None);
            await host.Stop();
        }

        [Fact]
        public async Task When_Create_PubSession_Then_NoError_IsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4009).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4009);
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
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4010).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4010);
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
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4011).AddVpns(new List<Vpn>
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
            var client = new EventMeshClient(port : 4011);
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
            var serviceProvider = new ServiceCollection().AddEventMeshServer(o => o.Port = 4012).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider;
            var host = serviceProvider.GetRequiredService<INodeHost>();
            var clientSessionStore = serviceProvider.GetRequiredService<IClientSessionStore>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4012);
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
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4013).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4013);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.AddBridge(vpn.Name, "urn", 6000, vpn.Name, "clientId", CancellationToken.None));
            await host.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.TARGET_NOT_REACHABLE, exception.Error);
        }

        [Fact]
        public async Task When_AddBridge_And_TargetVpnIsUnknown_Then_ErrorIsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var secondVpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
            var secondNewClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
            var firstHost = new ServiceCollection().AddEventMeshServer(o => o.Port = 4014).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            var secondHost = new ServiceCollection().AddEventMeshServer(n => n.Port = 6000).AddVpns(new List<Vpn>
            {
                secondVpn
            }).AddClients(new List<Models.Client> { secondNewClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await firstHost.Start(CancellationToken.None);
            await secondHost.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port : 4014);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.AddBridge(vpn.Name, "localhost", 6000, "invalidTargetVpn", "clientId", CancellationToken.None));
            await firstHost.Stop();
            await secondHost.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.UNKNOWN_TARGET_VPN, exception.Error);
        }

        [Fact]
        public async Task When_AddBridge_And_SourceVpnIsUnknown_Then_ErrorIsReturned()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var secondVpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
            var secondNewClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB });
            var firstHost = new ServiceCollection().AddEventMeshServer(o => o.Port = 4015).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            var secondHost = new ServiceCollection().AddEventMeshServer(n => n.Port = 6001).AddVpns(new List<Vpn>
            {
                secondVpn
            }).AddClients(new List<Models.Client> { secondNewClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await firstHost.Start(CancellationToken.None);
            await secondHost.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port : 4015);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.AddBridge("invalidTargetVpn", "localhost", 6001, secondVpn.Name, "clientId", CancellationToken.None));
            await firstHost.Stop();
            await secondHost.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.UNKNOWN_SOURCE_VPN, exception.Error);
        }

        #endregion

        #region Subscribe

        [Fact]
        public async Task When_Subscribe_ToOneSpecificTopic_Then_MessageIsReturned()
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
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB, UserAgentPurpose.SUB });
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4016).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            CloudEvent receivedCloudEvt = null;
            var client = new EventMeshClient(port: 4016);
            var subSession = await client.CreateSubSession(vpn.Name, newClient.ClientId, null, CancellationToken.None);
            subSession.DirectSubscribe("person.created", (cb) =>
            {
                receivedCloudEvt = cb;
            }, CancellationToken.None);
            var pubSession = await client.CreatePubSession(vpn.Name, newClient.ClientId, null, CancellationToken.None);
            await pubSession.Publish("person.created", cloudEvent, CancellationToken.None);
            while (receivedCloudEvt == null) Thread.Sleep(500);
            await host.Stop();

            // ASSERT
            Assert.NotNull(receivedCloudEvt);
            Assert.Equal("person.created", receivedCloudEvt.Type);
        }

        [Fact]
        public async Task When_Subscribe_To_WildCardTopic_Then_MessageIsReturned()
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
            var vpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB, UserAgentPurpose.SUB });
            var host = new ServiceCollection().AddEventMeshServer(o => o.Port = 4017).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient }).ServiceProvider.GetRequiredService<INodeHost>();
            await host.Start(CancellationToken.None);

            // ACT
            CloudEvent receivedCloudEvt = null;
            var client = new EventMeshClient(port: 4017);
            var subSession = await client.CreateSubSession(vpn.Name, newClient.ClientId, null, CancellationToken.None);
            await subSession.PersistedSubscribe("person.*", "groupId", (cb) =>
            {
                receivedCloudEvt = cb;
            }, CancellationToken.None);
            var pubSession = await client.CreatePubSession(vpn.Name, newClient.ClientId, null, CancellationToken.None);
            await pubSession.Publish("person.created", cloudEvent, CancellationToken.None);
            while (receivedCloudEvt == null) Thread.Sleep(500);
            await host.Stop();

            // ASSERT
            Assert.NotNull(receivedCloudEvt);
            Assert.Equal("person.created", receivedCloudEvt.Type);
        }

        // [Fact]
        public async Task When_Add_VpnBridge_BetweenTwoServers_Then_MessageCanBeTransmitted()
        {
            // ARRANGE
            var vpn = Vpn.Create("default", "default");
            var secondVpn = Vpn.Create("default", "default");
            var newClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB, UserAgentPurpose.SUB });
            var secondNewClient = Models.Client.Create(vpn.Name, "clientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB, UserAgentPurpose.SUB });
            var firstPubClient = Models.Client.Create(vpn.Name, "publishClientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB, UserAgentPurpose.SUB });
            var secondPubClient = Models.Client.Create(vpn.Name, "publishClientId", "urn", new List<UserAgentPurpose> { UserAgentPurpose.PUB, UserAgentPurpose.SUB });
            var firstHost = new ServiceCollection().AddEventMeshServer(o => o.Port = 4018).AddVpns(new List<Vpn>
            {
                vpn
            }).AddClients(new List<Models.Client> { newClient, firstPubClient }).ServiceProvider.GetRequiredService<INodeHost>();
            var secondHost = new ServiceCollection().AddEventMeshServer(n => n.Port = 6002).AddVpns(new List<Vpn>
            {
                secondVpn
            }).AddClients(new List<Models.Client> { secondNewClient, secondPubClient }).ServiceProvider.GetRequiredService<INodeHost>();
            var vpnBridgeSink = new ServiceCollection().AddVpnBridgeSeed(o => o.EventMeshPort = 4018).BuildServiceProvider().GetRequiredService<ISinkJob>();
            await firstHost.Start(CancellationToken.None);
            await secondHost.Start(CancellationToken.None);
            await vpnBridgeSink.Start(CancellationToken.None);

            // ACT
            CloudEvent receivedCloudEvt = null;
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
            var firstClient = new EventMeshClient(port: 4018);
            await firstClient.AddBridge("default", "localhost", 6002, "default", "clientId", CancellationToken.None);
            var subSession = await firstClient.CreateSubSession("default", "clientId", null, CancellationToken.None);
            subSession.DirectSubscribe("person.created", (ce) =>
            {
                receivedCloudEvt = ce;
            }, CancellationToken.None);
            var secondClient = new EventMeshClient(port: 6002);
            var pubSession = await secondClient.CreatePubSession("default", "clientId", null, CancellationToken.None);
            await pubSession.Publish("person.created", cloudEvent, CancellationToken.None);
            while (receivedCloudEvt == null) Thread.Sleep(500);
            await firstHost.Stop();
            await secondHost.Stop();
            await vpnBridgeSink.Stop();

            // ASSERT
            Assert.NotNull(receivedCloudEvt);
            Assert.Equal("person.created", receivedCloudEvt.Type);
        }

        #endregion

        #region Add VPN

        [Fact]
        public async Task When_AddSameVpnTwice_Then_ErrorIsReturned()
        {
            // ARRANGE
            var firstHost = new ServiceCollection().AddEventMeshServer(o => o.Port = 4019).ServiceProvider.GetRequiredService<INodeHost>();
            await firstHost.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4019);
            await client.AddVpn("default", CancellationToken.None);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.AddVpn("default"));
            await firstHost.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.VPN_ALREADY_EXISTS, exception.Error);
        }

        #endregion

        #region Add Client

        [Fact]
        public async Task When_AddClientAndVpnDoesntExist_Then_ErrorIsReturned()
        {
            // ARRANGE
            var firstHost = new ServiceCollection().AddEventMeshServer(o => o.Port = 4020).ServiceProvider.GetRequiredService<INodeHost>();
            await firstHost.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4020);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.AddClient("default", "clientId", new List<UserAgentPurpose> { }, CancellationToken.None));
            await firstHost.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.UNKNOWN_VPN, exception.Error);
        }

        [Fact]
        public async Task When_AddSameClientTwice_Then_ErrorIsReturned()
        {
            // ARRANGE
            var firstHost = new ServiceCollection().AddEventMeshServer(o => o.Port = 4021).ServiceProvider.GetRequiredService<INodeHost>();
            await firstHost.Start(CancellationToken.None);

            // ACT
            var client = new EventMeshClient(port: 4021);
            await client.AddVpn("default", CancellationToken.None);
            await client.AddClient("default", "clientId", new List<UserAgentPurpose> { }, CancellationToken.None);
            var exception = await Assert.ThrowsAsync<RuntimeClientResponseException>(async () => await client.AddClient("default", "clientId", new List<UserAgentPurpose> { }, CancellationToken.None));
            await firstHost.Stop();

            // ASSERT
            Assert.NotNull(exception);
            Assert.Equal(HeaderStatus.FAIL, exception.Status);
            Assert.Equal(Errors.CLIENT_ALREADY_EXISTS, exception.Error);
        }

        #endregion
    }
}
