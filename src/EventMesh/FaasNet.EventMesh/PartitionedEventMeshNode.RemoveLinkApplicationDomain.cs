using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.Client.StateMachines.Subscription;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(RemoveLinkApplicationDomainRequest removeLinkApplicationDomain, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = removeLinkApplicationDomain.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.RemoveLinkApplicationDomain(removeLinkApplicationDomain.Seq, RemoveLinkApplicationDomainStatus.UNKNOWN_VPN);
            var evtDef = await Query<GetApplicationDomainQueryResult>(PartitionNames.APPLICATION_DOMAIN, new  GetApplicationDomainQuery { Name = removeLinkApplicationDomain.Name, Vpn = removeLinkApplicationDomain.Vpn }, cancellationToken);
            if (!evtDef.Success) return PackageResponseBuilder.RemoveLinkApplicationDomain(removeLinkApplicationDomain.Seq, RemoveLinkApplicationDomainStatus.NOT_FOUND);
            var result = await Send(PartitionNames.APPLICATION_DOMAIN, new RemoveApplicationDomainLinkCommand { Name = removeLinkApplicationDomain.Name, Vpn = removeLinkApplicationDomain.Vpn, EventId = removeLinkApplicationDomain.EventId, Source = removeLinkApplicationDomain.Source, Target = removeLinkApplicationDomain.Target }, cancellationToken);
            await EnrichClients(removeLinkApplicationDomain, cancellationToken);
            await EnrichEventDefinition(removeLinkApplicationDomain, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.RemoveLinkApplicationDomain(removeLinkApplicationDomain.Seq, RemoveLinkApplicationDomainStatus.NOLEADER);
            return PackageResponseBuilder.RemoveLinkApplicationDomain(removeLinkApplicationDomain.Seq, RemoveLinkApplicationDomainStatus.OK);

            async Task EnrichClients(RemoveLinkApplicationDomainRequest removeLinkApplicationDomain, CancellationToken cancellationToken)
            {
                var cmds = new List<ICommand>
                {
                    new RemoveTargetCommand
                    {
                        ClientId = removeLinkApplicationDomain.Source,
                        Vpn = removeLinkApplicationDomain.Vpn,
                        Target = removeLinkApplicationDomain.Target,
                        EventDefId = removeLinkApplicationDomain.EventId
                    },
                    new RemoveSourceCommand
                    {
                        ClientId = removeLinkApplicationDomain.Target,
                        Vpn = removeLinkApplicationDomain.Vpn,
                        Source = removeLinkApplicationDomain.Source,
                        EventDefId = removeLinkApplicationDomain.EventId
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

            async Task EnrichEventDefinition(RemoveLinkApplicationDomainRequest removeLinkApplicationDomain, CancellationToken cancellationToken)
            {
                await Send(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new RemoveLinkEventDefinitionCommand
                {
                    Id = removeLinkApplicationDomain.EventId,
                    Source = removeLinkApplicationDomain.Source,
                    Target = removeLinkApplicationDomain.Target,
                    Vpn = removeLinkApplicationDomain.Vpn
                }, CancellationToken.None);
            }

            async Task EnrichSubscription(RemoveLinkApplicationDomainRequest removeLinkApplicationDomain, CancellationToken cancellationToken)
            {
                await Send(PartitionNames.SUBSCRIPTION_PARTITION_KEY, new RemoveSubscriptionCommand { ClientId = removeLinkApplicationDomain.Target, EventId = removeLinkApplicationDomain.EventId, Vpn = removeLinkApplicationDomain.Vpn }, cancellationToken);
            }
        }
    }
}
