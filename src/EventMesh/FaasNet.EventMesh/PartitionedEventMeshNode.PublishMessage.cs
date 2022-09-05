using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(PublishMessageRequest request, CancellationToken cancellationToken)
        {
            var partition = await PartitionPeerStore.Get(request.Topic);
            if (partition == null) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.UNKNOWN_TOPIC);
            var addMessageCommand = new AddVpnMessageCommand { Message = request.CloudEvent };
            var messageId = Guid.NewGuid().ToString();
            await Send(request.Topic, messageId, addMessageCommand, cancellationToken);
            return PackageResponseBuilder.PublishMessage(request.Seq);
        }
    }
}
