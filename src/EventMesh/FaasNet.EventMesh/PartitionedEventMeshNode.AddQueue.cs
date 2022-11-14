using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddQueueRequest addQueueRequest, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = addQueueRequest.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.AddQueue(addQueueRequest.Seq, AddQueueStatus.UNKNOWN_VPN);
            var queue = await Query<GetQueueQueryResult>(PartitionNames.QUEUE_PARTITION_KEY, new GetQueueQuery 
            { 
                QueueName = addQueueRequest.QueueName,
                Vpn = addQueueRequest.Vpn
            }, cancellationToken);
            if (queue.Success) return PackageResponseBuilder.AddQueue(addQueueRequest.Seq, AddQueueStatus.EXISTING_QUEUE);
            var addQueueCommand = new AddQueueCommand { QueueName = addQueueRequest.QueueName, Vpn = addQueueRequest.Vpn };
            await Send(PartitionNames.QUEUE_PARTITION_KEY, addQueueCommand, cancellationToken);
            return  PackageResponseBuilder.AddQueue(addQueueRequest.Seq);
        }
    }
}
