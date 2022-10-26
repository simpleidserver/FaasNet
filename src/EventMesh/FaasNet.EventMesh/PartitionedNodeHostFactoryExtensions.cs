using FaasNet.EventMesh;
using FaasNet.EventMesh.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.StateMachines.Client;
using FaasNet.EventMesh.StateMachines.EventDefinition;
using FaasNet.EventMesh.StateMachines.Queue;
using FaasNet.EventMesh.StateMachines.Subscriptions;
using FaasNet.Peer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace FaasNet.Partition
{
    public static class PartitionedNodeHostFactoryExtensions
    {
        public static PartitionedNodeHostFactory UseEventMesh(this PartitionedNodeHostFactory partitionedNodeHostFactory, Action<EventMeshOptions> callbackOpts = null)
        {
            RemovePartitionPeerFactory(partitionedNodeHostFactory);
            RemovePeerHost(partitionedNodeHostFactory);
            partitionedNodeHostFactory.Services.AddTransient<IPartitionPeerFactory, EventMeshPartitionPeerFactory>();
            partitionedNodeHostFactory.Services.AddScoped<IPeerHost, PartitionedEventMeshNode>();
            partitionedNodeHostFactory.Services.AddTransient<IMediator, Mediator>();
            partitionedNodeHostFactory.Services.AddTransient<IConsumer<ApplicationDomainLinkAdded>, EventDefinitionConsumer>();
            partitionedNodeHostFactory.Services.AddTransient<IConsumer<ApplicationDomainLinkRemoved>, EventDefinitionConsumer>();
            partitionedNodeHostFactory.Services.AddTransient<IConsumer<ApplicationDomainLinkAdded>, ClientConsumer>();
            partitionedNodeHostFactory.Services.AddTransient<IConsumer<ApplicationDomainLinkRemoved>, ClientConsumer>();
            partitionedNodeHostFactory.Services.AddTransient<IConsumer<ApplicationDomainLinkAdded>, SubscriptionConsumer>();
            partitionedNodeHostFactory.Services.AddTransient<IConsumer<ApplicationDomainLinkRemoved>, SubscriptionConsumer>();
            partitionedNodeHostFactory.Services.AddTransient<IConsumer<ClientAdded>, QueueConsumer>();
            if (callbackOpts != null) partitionedNodeHostFactory.Services.Configure(callbackOpts);
            else partitionedNodeHostFactory.Services.Configure<EventMeshOptions>(o => { });
            return partitionedNodeHostFactory;
        }

        private static void RemovePartitionPeerFactory(PartitionedNodeHostFactory partitionedNodeHostFactory) => RemoveDependency<IPartitionPeerFactory>(partitionedNodeHostFactory);
        private static void RemovePeerHost(PartitionedNodeHostFactory partitionedNodeHostFactory) => RemoveDependency<IPeerHost>(partitionedNodeHostFactory);

        private static void RemoveDependency<T>(PartitionedNodeHostFactory partitionedNodeHostFactory)
        {
            var registeredType = partitionedNodeHostFactory.Services.FirstOrDefault(s => s.ServiceType == typeof(T));
            if (registeredType != null) partitionedNodeHostFactory.Services.Remove(registeredType);
        }
    }
}
