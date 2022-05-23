using FaasNet.EventMesh.Runtime;
using FaasNet.EventMesh.Runtime.Handlers;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEventMeshServer(this IServiceCollection services, Action<ConsensusNodeOptions> consensusNodeOptions, Action<ConsensusPeerOptions> consensusPeerOptions = null, Action<EventMeshNodeOptions> callback = null)
        {
            if (callback != null) services.Configure(callback);
            services.Configure<EventMeshNodeOptions>(opt => { });
            services.AddLogging();
            services.RegisterConsensusPeer(consensusNodeOptions, consensusPeerOptions);
            services.AddTransient<INodeHost, EventMeshNode>();
            services.AddTransient<IBridgeServerStore, BridgeServerStore>();
            services.AddTransient<IClientSessionStore, ClientSessionStore>();
            services.AddTransient<IClientStore, ClientStore>();
            services.AddTransient<IMessageExchangeStore, MessageExchangeStore>();
            services.AddTransient<IVpnStore, VpnStore>();
            services.AddTransient<IQueueStore, QueueStore>();
            services.AddTransient<IMessageHandler, AddBridgeMessageHandler>();
            services.AddTransient<IMessageHandler, AddClientMessageHandler>();
            services.AddTransient<IMessageHandler, DisconnectMessageHandler>();
            services.AddTransient<IMessageHandler, GetAllVpnsMessageHandler>();
            services.AddTransient<IMessageHandler, HeartbeatMessageHandler>();
            services.AddTransient<IMessageHandler, HelloMessageHandler>();
            services.AddTransient<IMessageHandler, PublishMessageRequestHandler>();
            services.AddTransient<IMessageHandler, SubscribeMessageHandler>();
            services.AddTransient<IMessageHandler, AddVpnMessageHandler>();
            services.AddTransient<IMessageHandler, ReadNextMessageHandler>();
            services.AddTransient<IMessageHandler, ReadTopicMessageHandler>();
            services.AddScoped<IPeerHost, EventMeshPeer>();
            return services;
        }
    }
}
