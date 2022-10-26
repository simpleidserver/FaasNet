using FaasNet.EventMesh.Client.StateMachines.Queue;
using FaasNet.EventMesh.StateMachines.Client;
using FaasNet.Partition;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Queue
{
    public class QueueConsumer : BaseIntegrationEventConsumer, IConsumer<ClientAdded>
    {
        public QueueConsumer(IPartitionCluster partitionCluster) : base(partitionCluster)
        {
        }

        public async Task Consume(ClientAdded request, CancellationToken cancellationToken)
        {
            await Send(PartitionNames.QUEUE_PARTITION_KEY, new AddQueueCommand
            {
                QueueName = request.ClientId,
                Vpn = request.Vpn
            }, CancellationToken.None);
        }
    }
}
