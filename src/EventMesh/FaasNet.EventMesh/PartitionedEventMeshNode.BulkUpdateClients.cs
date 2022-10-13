using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(BulkUpdateClientRequest updateClientRequest, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(VPN_PARTITION_KEY, new GetVpnQuery { Id = updateClientRequest.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.BulkUpdateClient(updateClientRequest.Seq, UpdateClientErrorStatus.UNKNOWN_VPN);
            var clientSecret = Guid.NewGuid().ToString();
            var updateClientCommand = new BulkUpdateClientCommand
            {
                Vpn = updateClientRequest.Vpn,
                Clients = updateClientRequest.Clients.Select(c => new UpdateClient 
                {
                    Id = c.Id, 
                    CoordinateX = c.CoordinateX, 
                    CoordinateY = c.CoordinateY, 
                    Targets = c.Targets.Select(t => new ClientTargetResult
                    {
                        EventId = t.EventId,
                        Target = t.Target
                    }).ToList()
                }).ToList()
            };
            var result = await Send(CLIENT_PARTITION_KEY, updateClientCommand, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.BulkUpdateClient(updateClientRequest.Seq, UpdateClientErrorStatus.NOLEADER);
            return PackageResponseBuilder.BulkUpdateClient(updateClientRequest.Seq);
        }
    }
}
