using FaasNet.Peer.Client.Messages;
using FaasNet.Peer.Client.Transports;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Client
{
    public class PartitionClient : BasePeerClient
    {
        public PartitionClient(IClientTransport clientTransort) : base(clientTransort) { }

        public async Task<AddDirectPartitionResult> AddPartition(string partitionKey, Type partitionType, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeBuffer = new WriteBufferContext();
            var pkg = PartitionPackageRequestBuilder.AddPartition(partitionKey, partitionType.AssemblyQualifiedName);
            pkg.SerializeEnvelope(writeBuffer);
            await Send(writeBuffer.Buffer.ToArray(), timeoutMS, cancellationToken);
            var receivedResult = await Receive(timeoutMS, cancellationToken);
            var readBuffer = new ReadBufferContext(receivedResult);
            return BasePartitionedRequest.Deserialize(readBuffer) as AddDirectPartitionResult;
        }

        public async Task<RemoveDirectPartitionResult> RemovePartition(string partitionKey, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeBuffer = new WriteBufferContext();
            var pkg = PartitionPackageRequestBuilder.RemovePartition(partitionKey);
            pkg.SerializeEnvelope(writeBuffer);
            await Send(writeBuffer.Buffer.ToArray(), timeoutMS, cancellationToken);
            var receivedResult = await Receive(timeoutMS, cancellationToken);
            var readBuffer = new ReadBufferContext(receivedResult);
            return BasePartitionedRequest.Deserialize(readBuffer) as RemoveDirectPartitionResult;
        }

        public async Task<GetAllNodesResult> GetAllNodes(int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeBuffer = new WriteBufferContext();
            var pkg = PartitionPackageRequestBuilder.GetAllNodes();
            pkg.SerializeEnvelope(writeBuffer);
            await Send(writeBuffer.Buffer.ToArray(), timeoutMS, cancellationToken);
            var receivedResult = await Receive(timeoutMS, cancellationToken);
            var readBuffer = new ReadBufferContext(receivedResult);
            return BasePartitionedRequest.Deserialize(readBuffer) as GetAllNodesResult;
        }
    }
}
