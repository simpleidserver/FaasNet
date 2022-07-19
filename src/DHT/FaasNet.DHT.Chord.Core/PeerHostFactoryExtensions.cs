using FaasNet.DHT.Chord.Core;
using FaasNet.DHT.Chord.Core.Handlers;
using FaasNet.DHT.Chord.Core.Stores;
using FaasNet.Peer;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class PeerHostFactoryExtensions
    {
        public static PeerHostFactory AddDHTChordProtocol(this PeerHostFactory peerHostFactory, Action<ChordOptions> options = null)
        {
            if (options == null) peerHostFactory.Services.Configure<ChordOptions>(o => { });
            else peerHostFactory.Services.Configure(options);
            peerHostFactory.Services.AddTransient<IProtocolHandler, ChordProtocolHandler>();
            peerHostFactory.Services.AddTransient<ITimer, ChordTimer>();
            peerHostFactory.Services.AddTransient<IRequestHandler, GetDimensionFingerTableRequestHandler>();
            peerHostFactory.Services.AddTransient<IRequestHandler, JoinChordNetworkRequestHandler>();
            peerHostFactory.Services.AddTransient<IRequestHandler, FindSuccessorRequestHandler>();
            peerHostFactory.Services.AddTransient<IRequestHandler, CreateRequestHandler>();
            peerHostFactory.Services.AddTransient<IRequestHandler, NotifyRequestHandler>();
            peerHostFactory.Services.AddTransient<IRequestHandler, FindPredecessorRequestHandler>();
            peerHostFactory.Services.AddTransient<IRequestHandler, GetKeyRequestHandler>();
            peerHostFactory.Services.AddTransient<IRequestHandler, AddKeyRequestHandler>();
            peerHostFactory.Services.AddScoped<IDHTPeerInfoStore, DHTPeerInfoStore>();
            peerHostFactory.Services.AddScoped<IPeerDataStore, PeerDataStore>();
            return peerHostFactory;
        }
    }
}
