using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class AddBridgeMessageHandler : IMessageHandler
    {
        private readonly IUdpClientServerFactory _udpClientFactory;
        private readonly IVpnStore _vpnStore;

        public AddBridgeMessageHandler(
            IUdpClientServerFactory udpClientFactory,
            IVpnStore vpnStore)
        {
            _udpClientFactory = udpClientFactory;
            _vpnStore = vpnStore;
        }

        public Commands Command => Commands.ADD_BRIDGE_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var addBridgeRequest = package as AddBridgeRequest;
            var vpn = await _vpnStore.Get(addBridgeRequest.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new RuntimeException(addBridgeRequest.Header.Command, addBridgeRequest.Header.Seq, Errors.UNKNOWN_VPN);
            }

            var bridgeServer = vpn.GetBridgeServer(addBridgeRequest.TargetUrn, addBridgeRequest.TargetPort, addBridgeRequest.TargetVpn);
            if (bridgeServer != null)
            {
                throw new RuntimeException(addBridgeRequest.Header.Command, addBridgeRequest.Header.Seq, Errors.BRIDGE_EXISTS);
            }

            var udpClient = _udpClientFactory.Build();
            try
            {
                var runtimeClient = new RuntimeClient(udpClient, addBridgeRequest.TargetUrn, addBridgeRequest.TargetPort);
                await runtimeClient.HeartBeat();
            }
            catch(RuntimeClientException)
            {
                throw new RuntimeException(addBridgeRequest.Header.Command, addBridgeRequest.Header.Seq, Errors.INVALID_BRIDGE);
            }

            vpn.AddBridge(addBridgeRequest.TargetUrn, addBridgeRequest.TargetPort, addBridgeRequest.TargetVpn);
            return PackageResponseBuilder.AddBridge(package.Header.Seq);
        }
    }
}
