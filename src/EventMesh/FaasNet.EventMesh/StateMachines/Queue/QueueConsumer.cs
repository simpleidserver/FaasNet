using FaasNet.EventMesh.StateMachines.QueueMessage;
using FaasNet.Partition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Queue
{
    public class QueueConsumer : BaseIntegrationEventConsumer, IConsumer<QueueAdded>
    {
        public QueueConsumer(IPartitionCluster partitionCluster) : base(partitionCluster)
        {
        }

        public async Task Consume(QueueAdded request, CancellationToken cancellationToken)
        {
            Debug.WriteLine($"{request.Vpn}_{request.QueueName}");
            await PartitionCluster.TryAddAndStart($"{request.Vpn}_{request.QueueName}", typeof(QueueMessageStateMachine));
        }
    }
}
