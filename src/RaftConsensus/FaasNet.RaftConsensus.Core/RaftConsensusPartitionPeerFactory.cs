﻿using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace FaasNet.RaftConsensus.Core
{
    public class RaftConsensusPartitionPeerFactory : IPartitionPeerFactory
    {
        private readonly IClusterStore _clusterStore;
        private readonly RaftConsensusPeerOptions _options;

        public RaftConsensusPartitionPeerFactory(IClusterStore clusterStore, IOptions<RaftConsensusPeerOptions> options)
        {
            _clusterStore = clusterStore;
            _options = options.Value;
        }

        public IPeerHost Build(int port, string partitionKey, Type stateMachineType, IMediator mediator, Action<IServiceCollection> callbackService = null, Action<PeerHostFactory> callbackHostFactory = null)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var hostFactory = PeerHostFactory.NewUnstructured(o => {
                o.Port = port;
                o.PartitionKey = partitionKey;
            }, callbackService: (s) =>
            {
                s.AddSingleton(mediator);
                if (callbackService != null) callbackService(s);
            })
                .UseServerUDPTransport()
                .UseClientUDPTransport()
                .UseClusterStore(_clusterStore)
                .AddRaftConsensus(o =>
                {
                    o.ConfigurationDirectoryPath = Path.Combine(path, port.ToString());
                    o.StateMachineType = stateMachineType ?? _options.StateMachineType;
                    o.LeaderCallback += (s) =>
                    {
                        Debug.WriteLine($"There is one leader for the partition {s} !");
                    };
                });
            if (callbackHostFactory != null) callbackHostFactory(hostFactory);
            return hostFactory.Build();
        }
    }
}
