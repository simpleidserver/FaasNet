using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Infos;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Peer
{
    public static class PeerHostFactoryExtensions
    {
        public static PeerHostFactory AddRaftConsensus(this PeerHostFactory peerHostFactory, Action<RaftConsensusPeerOptions> options = null)
        {
            if (options == null) peerHostFactory.Services.Configure<RaftConsensusPeerOptions>(o => { });
            else peerHostFactory.Services.Configure(options);
            peerHostFactory.Services.AddScoped<ILogStore, InMemoryLogStore>();
            peerHostFactory.Services.AddScoped<IPeerInfoStore, PeerInfoStore>();
            peerHostFactory.Services.AddTransient<ITimer, RaftConsensusTimer>();
            peerHostFactory.Services.AddTransient<IProtocolHandler, RaftConsensusProtocolHandler>();
            return peerHostFactory;
        }
    }
}
