using FaasNet.Partition;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Messages;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines
{
    public class BaseIntegrationEventConsumer
    {
        public BaseIntegrationEventConsumer(IPartitionCluster partitionCluster)
        {
            PartitionCluster = partitionCluster;
        }

        protected IPartitionCluster PartitionCluster { get; private set; }

        protected async Task<AppendEntryResult> Send(string partitionKey, ICommand command, CancellationToken cancellationToken)
        {
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.AppendEntry(CommandSerializer.Serialize(command)).SerializeEnvelope(writeBufferContext);
            var cmdBuffer = writeBufferContext.Buffer.ToArray();
            var transferedResult = await PartitionCluster.Transfer(new TransferedRequest
            {
                Content = cmdBuffer,
                PartitionKey = partitionKey
            }, cancellationToken);
            var readBufferContext = new ReadBufferContext(transferedResult);
            var result = BaseConsensusPackage.Deserialize(readBufferContext) as AppendEntryResult;
            return result;
        }

        protected async Task<T> Query<T>(string partitionKey, IQuery query, CancellationToken cancellationToken) where T : IQueryResult
        {
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.Query(query).SerializeEnvelope(writeBufferContext);
            var content = writeBufferContext.Buffer.ToArray();
            var transferedResult = await PartitionCluster.Transfer(new TransferedRequest
            {
                PartitionKey = partitionKey,
                Content = content
            }, cancellationToken);
            var readBufferContext = new ReadBufferContext(transferedResult);
            var stateMachineResult = BaseConsensusPackage.Deserialize(readBufferContext) as QueryResult;
            return (T)stateMachineResult.Result;
        }
    }
}
