using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace FaasNet.Peer
{
    public static class PeerHostFactoryExtensions
    {
        public static PeerHostFactory AddRaftConsensus(this PeerHostFactory peerHostFactory, Action<RaftConsensusPeerOptions> options = null, ConcurrentBag<PartitionElectionRecord> partitionElectionRecords = null)
        {
            if (options == null) peerHostFactory.Services.Configure<RaftConsensusPeerOptions>(o => { });
            else peerHostFactory.Services.Configure(options);
            if (partitionElectionRecords == null) peerHostFactory.Services.AddSingleton<IPartitionElectionStore, InMemoryPartitionElectionStore>();
            else peerHostFactory.Services.AddSingleton<IPartitionElectionStore>(new InMemoryPartitionElectionStore(partitionElectionRecords));
            peerHostFactory.Services.AddSingleton<ILeaderPeerInfoStore, InMemoryLeaderPeerInfoStore>();
            peerHostFactory.Services.AddSingleton<ILogStore, InMemoryLogStore>();
            peerHostFactory.Services.AddTransient<ITimer, RaftConsensusTimer>();
            peerHostFactory.Services.AddTransient<IProtocolHandler, RaftConsensusProtocolHandler>();
            return peerHostFactory;
        }
    }
}
