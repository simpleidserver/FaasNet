using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddApplicationDomainRequest addApplicationDomain, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = addApplicationDomain.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.AddApplicationDomain(addApplicationDomain.Seq, AddApplicationDomainStatus.UNKNOWN_VPN);
            var addApplicationDomainCmd = new AddApplicationDomainCommand { Description = addApplicationDomain.Description, Name = addApplicationDomain.Name, RootTopic = addApplicationDomain.RootTopic, Vpn = addApplicationDomain.Vpn };
            var result = await Send(PartitionNames.APPLICATION_DOMAIN, addApplicationDomainCmd, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.AddApplicationDomain(addApplicationDomain.Seq, AddApplicationDomainStatus.NOLEADER);
            return PackageResponseBuilder.AddApplicationDomain(addApplicationDomain.Seq);
        }
    }
}
