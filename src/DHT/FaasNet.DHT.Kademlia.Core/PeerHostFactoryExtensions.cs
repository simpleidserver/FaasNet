using FaasNet.DHT.Kademlia.Core;
using FaasNet.DHT.Kademlia.Core.Handlers;
using FaasNet.DHT.Kademlia.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Peer
{
    public static class PeerHostFactoryExtensions
    {
        public static PeerHostFactory AddDHTKademliaProtocol(this PeerHostFactory serverBuilder, Action<KademliaOptions> callback = null)
        {
            if (callback != null) serverBuilder.Services.Configure(callback);
            else serverBuilder.Services.Configure<KademliaOptions>(o => { });
            serverBuilder.Services.AddLogging();
            serverBuilder.Services.AddTransient<IProtocolHandler, KademliaProtocolHandler>();
            serverBuilder.Services.AddTransient<ITimer, KademliaTimer>();
            serverBuilder.Services.AddTransient<IRequestHandler, FindNodeRequestHandler>();
            serverBuilder.Services.AddTransient<IRequestHandler, PingRequestHandler>();
            serverBuilder.Services.AddTransient<IRequestHandler, FindValueRequestHandler>();
            serverBuilder.Services.AddTransient<IRequestHandler, StoreRequestHandler>();
            serverBuilder.Services.AddScoped<IDHTPeerInfoStore, DHTPeerInfoStore>();
            serverBuilder.Services.AddScoped<IPeerDataStore, PeerDataStore>();
            return serverBuilder;
        }
    }
}
