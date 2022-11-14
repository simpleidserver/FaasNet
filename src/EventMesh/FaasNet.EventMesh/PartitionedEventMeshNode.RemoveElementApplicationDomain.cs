using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(RemoveElementApplicationDomainRequest removeEltApplicationDomain, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = removeEltApplicationDomain.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.RemoveApplicationDomainElement(removeEltApplicationDomain.Seq, RemoveElementApplicationDomainStatus.UNKNOWN_VPN);
            var appDomainResult = await Query<GetApplicationDomainQueryResult>(PartitionNames.APPLICATION_DOMAIN, new GetApplicationDomainQuery { Name = removeEltApplicationDomain.Name, Vpn = removeEltApplicationDomain.Vpn }, cancellationToken);
            var elt = appDomainResult.Content.Elements.Single(e => e.ElementId == removeEltApplicationDomain.ElementId);
            var removeApplicationDomainEltCmd = new RemoveApplicationDomainElementCommand { ElementId = removeEltApplicationDomain.ElementId, Name = removeEltApplicationDomain.Name, Vpn = removeEltApplicationDomain.Vpn };
            var result = await Send(PartitionNames.APPLICATION_DOMAIN, removeApplicationDomainEltCmd, cancellationToken);
            await EnrichClients(removeEltApplicationDomain, elt, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.RemoveApplicationDomainElement(removeEltApplicationDomain.Seq, RemoveElementApplicationDomainStatus.NOLEADER);
            return PackageResponseBuilder.RemoveApplicationDomainElement(removeEltApplicationDomain.Seq, RemoveElementApplicationDomainStatus.OK);

            async Task EnrichClients(RemoveElementApplicationDomainRequest removeEltApplicationDomain, ApplicationDomainElementResult elt, CancellationToken cancellationToken)
            {
                foreach (var link in elt.Targets)
                {
                    var cmds = new List<ICommand>
                {
                    new RemoveTargetCommand
                    {
                        ClientId = removeEltApplicationDomain.ElementId,
                        Vpn = removeEltApplicationDomain.Vpn,
                        Target = link.Target,
                        EventDefId = link.EventId
                    },
                    new RemoveSourceCommand
                    {
                        ClientId = link.Target,
                        Vpn = removeEltApplicationDomain.Vpn,
                        Source = removeEltApplicationDomain.ElementId,
                        EventDefId = link.EventId
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
            }
        }
    }
}
