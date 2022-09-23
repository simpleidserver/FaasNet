using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetClientRequest getClientRequest, CancellationToken cancellationToken)
        {
            var client = await Query<GetClientQueryResult>(CLIENT_PARTITION_KEY, new GetClientQuery { Id = getClientRequest.ClientId, Vpn = getClientRequest.Vpn }, cancellationToken);
            if (!client.Success) return null;
            return null;
    }
}
