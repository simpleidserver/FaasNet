using FaasNet.Peer.Client.Messages;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Client
{
    public class UDPPartitionClient : BasePeerClient, IPartitionClient
    {
        public UDPPartitionClient(IPEndPoint target) : base(target)
        {
            UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public UDPPartitionClient(string url, int port) : base(url, port) 
        {
            UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public UdpClient UdpClient { get; private set; }

        public async Task AddPartition(string partitionKey, CancellationToken cancellationToken)
        {
            var writeBuffer = new WriteBufferContext();
            var pkg = PartitionPackageRequestBuilder.AddPartition(partitionKey);
            pkg.SerializeEnvelope(writeBuffer);
            await UdpClient.SendAsync(writeBuffer.Buffer.ToArray(), Target, cancellationToken);
            await UdpClient.ReceiveAsync(cancellationToken);
        }

        public override void Dispose()
        {
            UdpClient?.Dispose();
        }
    }
}
