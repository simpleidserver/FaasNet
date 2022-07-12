using FaasNet.Common.Extensions;
using FaasNet.Common.Helpers;
using FaasNet.CRDT.Client.Messages;
using FaasNet.Peer.Client;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.CRDT.Client
{
    public class UDPCRDTClient : IDisposable
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private readonly UdpClient _udpClient;

        public UDPCRDTClient(string url, int port)
        {
            _ipAddress = DnsHelper.ResolveIPV4(url);
            _port = port;
            _udpClient = new UdpClient();
        }

        public async Task IncrementGCounter(string peerId, string entityId, long increment, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = CRDTPackageRequestBuilder.IncrementGCounter(peerId, entityId, increment, nonce);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddress, _port)).WithCancellation(cancellationToken, timeoutMS);
            var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = CRDTPackage.Deserialize(readCtx);
        }

        public void Dispose()
        {
            if (_udpClient != null)
            {
                _udpClient.Close();
                _udpClient.Dispose();
            }
        }
    }
}
