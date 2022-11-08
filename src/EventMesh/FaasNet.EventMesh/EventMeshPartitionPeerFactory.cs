﻿using FaasNet.EventMesh.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.StateMachines.Client;
using FaasNet.EventMesh.StateMachines.EventDefinition;
using FaasNet.EventMesh.StateMachines.Queue;
using FaasNet.EventMesh.StateMachines.QueueMessage;
using FaasNet.EventMesh.StateMachines.Session;
using FaasNet.EventMesh.StateMachines.Subscriptions;
using FaasNet.EventMesh.StateMachines.Vpn;
using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace FaasNet.EventMesh
{
    public class EventMeshPartitionPeerFactory : IPartitionPeerFactory
    {
        private readonly IClusterStore _clusterStore;
        private readonly RaftConsensusPeerOptions _options;

        public EventMeshPartitionPeerFactory(IClusterStore clusterStore, IOptions<RaftConsensusPeerOptions> options)
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
                s.AddScoped<IClientStateMachineStore, ClientStateMachineStore>();
                s.AddScoped<IQueueStateMachineStore, QueueStateMachineStore>();
                s.AddScoped<IQueueMessageStateMachineStore, QueueMessageStateMachineStore>();
                s.AddScoped<ISessionStateMachineStore, SessionStateMachineStore>();
                s.AddScoped<IVpnStateMachineStore, VpnStateMachineStore>();
                s.AddScoped<IEventDefinitionStateMachineStore, EventDefinitionStateMachineStore>();
                s.AddScoped<IApplicationDomainStateMachineStore, ApplicationDomainStateMachineStore>();
                s.AddScoped<ISubscriptionStateMachineStore, SubscriptionStateMachineStore>();
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
                    o.IsConfigurationStoredInMemory = true;
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
