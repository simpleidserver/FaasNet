using FaasNet.EventMesh;
using FaasNet.EventMesh.StateMachines.Client;
using FaasNet.EventMesh.StateMachines.EventDefinition;
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
            partitionedNodeHostFactory.Services.AddTransient<IConsumer<ClientRemoved>, EventDefinitionConsumer> ();
            partitionedNodeHostFactory.Services.AddTransient<IConsumer<EventDefinitionLinkAdded>, ClientConsumer>();
            partitionedNodeHostFactory.Services.AddTransient<IConsumer<EventDefinitionLinkRemoved>, ClientConsumer>();
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
