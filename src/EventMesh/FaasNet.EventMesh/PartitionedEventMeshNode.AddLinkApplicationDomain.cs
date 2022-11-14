using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.Client.StateMachines.Subscription;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using FaasNet.RaftConsensus.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddLinkApplicationDomainRequest addLinkApplicationDomain, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = addLinkApplicationDomain.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.AddLinkApplicationDomain(addLinkApplicationDomain.Seq, AddLinkApplicationDomainStatus.UNKNOWN_VPN);
            var evtDef = await Query<GetApplicationDomainQueryResult>(PartitionNames.APPLICATION_DOMAIN, new  GetApplicationDomainQuery { Name = addLinkApplicationDomain.Name, Vpn = addLinkApplicationDomain.Vpn }, cancellationToken);
            if (!evtDef.Success) return PackageResponseBuilder.AddLinkApplicationDomain(addLinkApplicationDomain.Seq, AddLinkApplicationDomainStatus.NOT_FOUND);
            var result = await Send(PartitionNames.APPLICATION_DOMAIN, new AddApplicationDomainLinkCommand { Name = addLinkApplicationDomain.Name, Vpn = addLinkApplicationDomain.Vpn, EventId = addLinkApplicationDomain.EventId, Source = addLinkApplicationDomain.Source, Target = addLinkApplicationDomain.Target }, cancellationToken);
            await EnrichClients(addLinkApplicationDomain, cancellationToken);
            await EnrichEventDefinition(addLinkApplicationDomain, cancellationToken);
            await EnrichSubscription(addLinkApplicationDomain, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.AddLinkApplicationDomain(addLinkApplicationDomain.Seq, AddLinkApplicationDomainStatus.NOLEADER);
            return PackageResponseBuilder.AddLinkApplicationDomain(addLinkApplicationDomain.Seq, AddLinkApplicationDomainStatus.OK);

            async Task EnrichClients(AddLinkApplicationDomainRequest addLinkApplicationDomain, CancellationToken cancellationToken)
            {
                var cmds = new List<ICommand>
                {
                    new AddTargetCommand
                    {
                        ClientId = addLinkApplicationDomain.Source,
                        Vpn = addLinkApplicationDomain.Vpn,
                        Target = addLinkApplicationDomain.Target,
                        EventDefId = addLinkApplicationDomain.EventId
                    },
                    new AddSourceCommand
                    {
                        ClientId = addLinkApplicationDomain.Target,
                        Vpn = addLinkApplicationDomain.Vpn,
                        Source = addLinkApplicationDomain.Source,
                        EventDefId = addLinkApplicationDomain.EventId
                    }
                };
                await Parallel.ForEachAsync(cmds, new ParallelOptions
                {
                    MaxDegreeOfParallelism = _options.MaxConcurrentThreads
                }, async (cmd, t) =>
                {
                    await Send(PartitionNames.CLIENT_PARTITION_KEY, cmd, CancellationToken.None);
                });
            }

            async Task EnrichEventDefinition(AddLinkApplicationDomainRequest addLinkApplicationDomain, CancellationToken cancellationToken)
            {
                await Send(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new AddLinkEventDefinitionCommand
                {
                    Id = addLinkApplicationDomain.EventId,
                    Source = addLinkApplicationDomain.Source,
                    Target = addLinkApplicationDomain.Target,
                    Vpn = addLinkApplicationDomain.Vpn
                }, CancellationToken.None);
            }

            async Task EnrichSubscription(AddLinkApplicationDomainRequest addLinkApplicationDomainRequest, CancellationToken cancellationToken)
            {
                var evt = await Query<GetEventDefinitionQueryResult>(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new GetEventDefinitionQuery { Id = addLinkApplicationDomainRequest.EventId, Vpn = addLinkApplicationDomainRequest.Vpn }, cancellationToken);
                if (!evt.Success) return;
                await Send(PartitionNames.SUBSCRIPTION_PARTITION_KEY, new AddSubscriptionCommand { ClientId = addLinkApplicationDomainRequest.Target, EventId = addLinkApplicationDomainRequest.EventId, Topic = evt.EventDef.Topic, Vpn = addLinkApplicationDomainRequest.Vpn, Id = Guid.NewGuid().ToString() }, cancellationToken);
            }
        }
    }
}
