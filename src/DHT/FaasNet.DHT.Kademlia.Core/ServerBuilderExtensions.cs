using FaasNet.DHT.Kademlia.Core;
using FaasNet.DHT.Kademlia.Core.Handlers;
using FaasNet.DHT.Kademlia.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder AddDHTKademlia(this ServerBuilder serverBuilder, Action<DHTOptions> callback = null)
        {
            if (callback != null) serverBuilder.Services.Configure(callback);
            else serverBuilder.Services.Configure<DHTOptions>(o => { });
            serverBuilder.Services.AddLogging();
            serverBuilder.Services.AddTransient<IDHTPeerFactory, DHTPeerFactory>();
            serverBuilder.Services.AddTransient<IDHTPeer, DHTPeer>();
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
