using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Messages;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddVpnRequest addVpnRequest, CancellationToken cancellationToken)
        {
            var addVpnCommand = new AddVpnRecordCommand { Record = new VpnRecord { Id = addVpnRequest.Vpn } };
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.AppendEntry(CommandSerializer.Serialize(addVpnCommand)).SerializeEnvelope(writeBufferContext);
            var cmdBuffer = writeBufferContext.Buffer.ToArray();
            await PartitionCluster.Transfer(new TransferedRequest
            {
                Content = cmdBuffer,
                PartitionKey = VPN_PARTITION_KEY
            }, cancellationToken);
            return PackageResponseBuilder.AddVpn(addVpnRequest.Seq);
        }
    }
}
