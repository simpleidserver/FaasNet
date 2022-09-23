using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.QueueMessage;
using FaasNet.EventMesh.Client.StateMachines.Session;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(ReadMessageRequest readMessageRequest, CancellationToken cancellationToken)
        {
            var session = await Query<GetSessionQueryResult>(SESSION_PARTITION_KEY, new GetSessionQuery { Id = readMessageRequest.SessionId }, cancellationToken);
            if (!session.Success) return PackageResponseBuilder.ReadMessage(readMessageRequest.Seq, ReadMessageStatus.UNKNOWN_SESSION);
            if (!session.Session.IsValid) return PackageResponseBuilder.ReadMessage(readMessageRequest.Seq, ReadMessageStatus.EXPIRED_SESSION);
            if (session.Session.ClientPurpose != ClientPurposeTypes.SUBSCRIBE) return PackageResponseBuilder.ReadMessage(readMessageRequest.Seq, ReadMessageStatus.BAD_SESSION_USAGE);
            var partitionKey = $"{session.Session.Vpn}_{session.Session.QueueName}";
            var stateMachine = await Query<GetQueueMessageQueryResult>(partitionKey, new GetQueueMessageQuery { Offset = readMessageRequest.Offset, QueueName = session.Session.QueueName }, cancellationToken);
            if (!stateMachine.Success) return PackageResponseBuilder.ReadMessage(readMessageRequest.Seq, ReadMessageStatus.NO_MESSAGE);
            return PackageResponseBuilder.ReadMessage(readMessageRequest.Seq, stateMachine.Message.Data);
        }
    }
}