using FaasNet.Peer.Client.Messages;
using FaasNet.Peer.Client.Transports;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Client
{
    public class PartitionClient : BasePeerClient
    {
        public PartitionClient(IClientTransport clientTransort) : base(clientTransort) { }

        public async Task AddPartition(string partitionKey, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeBuffer = new WriteBufferContext();
            var pkg = PartitionPackageRequestBuilder.AddPartition(partitionKey);
            pkg.SerializeEnvelope(writeBuffer);
            await Send(writeBuffer.Buffer.ToArray(), timeoutMS, cancellationToken);
            await Receive(timeoutMS, cancellationToken);
        }
    }
}
