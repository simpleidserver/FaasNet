using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(ReadMessageRequest readMessageRequest, CancellationToken cancellationToken)
        {
            var session = await GetStateMachine<SessionStateMachine>(SESSION_PARTITION_KEY, readMessageRequest.SessionId, cancellationToken);
            if (session == null) return PackageResponseBuilder.ReadMessage(readMessageRequest.Seq, ReadMessageStatus.UNKNOWN_SESSION);
            if (!session.IsValid) return PackageResponseBuilder.ReadMessage(readMessageRequest.Seq, ReadMessageStatus.EXPIRED_SESSION);
            if (session.ClientPurpose != ClientPurposeTypes.SUBSCRIBE) return PackageResponseBuilder.ReadMessage(readMessageRequest.Seq, ReadMessageStatus.BAD_SESSION_USAGE);
            var stateMachine = await ReadStateMachine<QueueMessageStateMachine>(session.QueueName, readMessageRequest.Offset, cancellationToken);
            if (stateMachine == null || stateMachine.Data == null) return PackageResponseBuilder.ReadMessage(readMessageRequest.Seq, ReadMessageStatus.NO_MESSAGE);
            return PackageResponseBuilder.ReadMessage(readMessageRequest.Seq, stateMachine.Data);
        }
    }
}