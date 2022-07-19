using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace FaasNet.Peer
{
    public static class PeerHostFactoryExtensions
    {
        public static PeerHostFactory AddRaftConsensus(this PeerHostFactory peerHostFactory, Action<RaftConsensusPeerOptions> options = null, ConcurrentBag<PartitionElectionRecord> partitionElectionRecords = null)
        {
            if (options == null) peerHostFactory.Services.Configure<RaftConsensusPeerOptions>(o => { });
            else peerHostFactory.Services.Configure(options);
            if (partitionElectionRecords == null) peerHostFactory.Services.AddScoped<IPartitionElectionStore, InMemoryPartitionElectionStore>();
            else peerHostFactory.Services.AddScoped<IPartitionElectionStore>(s => new InMemoryPartitionElectionStore(partitionElectionRecords));
            peerHostFactory.Services.AddScoped<ILogStore, InMemoryLogStore>();
            peerHostFactory.Services.AddScoped<ILeaderPeerInfoStore, InMemoryLeaderPeerInfoStore>();
            peerHostFactory.Services.AddScoped<IRaftConsensusPartitionTimerStore, RaftConsensusPartitionTimerStore>();
            peerHostFactory.Services.AddScoped<IPendingRequestStore, PendingRequestStore>();
            peerHostFactory.Services.AddTransient<ITimer, RaftConsensusTimer>();
            peerHostFactory.Services.AddTransient<IProtocolHandler, RaftConsensusProtocolHandler>();
            return peerHostFactory;
        }

        public static PeerHostFactory UseFileLogStore(this PeerHostFactory peerHostFactory, Action<FileLogStoreOptions> options = null)
        {
            RemoveLogStore(peerHostFactory.Services);
            if (options == null) peerHostFactory.Services.Configure<FileLogStoreOptions>(o => { });
            else peerHostFactory.Services.Configure(options);
            peerHostFactory.Services.AddTransient<ILogStore, FileLogStore>();
            return peerHostFactory;
        }

        private static void RemoveLogStore(IServiceCollection services)
        {
            var registeredType = services.FirstOrDefault(s => s.ServiceType == typeof(ILogStore));
            if (registeredType != null) services.Remove(registeredType);
        }
    }
}
