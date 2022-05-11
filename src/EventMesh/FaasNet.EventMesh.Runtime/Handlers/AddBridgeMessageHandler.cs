using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Exceptions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class AddBridgeMessageHandler : IMessageHandler
    {
        private readonly IVpnStore _vpnStore;
        private readonly IBridgeStore _bridgeStore;

        public AddBridgeMessageHandler(IVpnStore vpnStore, IBridgeStore bridgeStore)
        {
            _vpnStore = vpnStore;
            _bridgeStore = bridgeStore;
        }

        public Commands Command => Commands.ADD_BRIDGE_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, CancellationToken cancellationToken)
        {
            var addBridgeRequest = package as AddBridgeRequest;
            await CheckVpn(addBridgeRequest, cancellationToken);
            await CheckBridgeServer(addBridgeRequest, cancellationToken);
            await _bridgeStore.Add(BridgeServer.Create(addBridgeRequest.Vpn, addBridgeRequest.TargetUrn, addBridgeRequest.TargetPort, addBridgeRequest.TargetVpn), cancellationToken);
            return EventMeshPackageResult.SendResult(PackageResponseBuilder.AddBridge(package.Header.Seq));
        }

        private async Task CheckVpn(AddBridgeRequest addBridgeRequest, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(addBridgeRequest.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new RuntimeException(addBridgeRequest.Header.Command, addBridgeRequest.Header.Seq, Errors.UNKNOWN_VPN);
            }
        }

        private async Task CheckBridgeServer(AddBridgeRequest addBridgeRequest, CancellationToken cancellationToken)
        {
            var bridgeServers = await _bridgeStore.GetAll(cancellationToken);
            if (bridgeServers.Any(b => b.SourceVpn == addBridgeRequest.Vpn && b.TargetUrn == addBridgeRequest.TargetUrn && b.TargetPort == addBridgeRequest.TargetPort && b.TargetVpn == addBridgeRequest.TargetVpn))
            {
                throw new RuntimeException(addBridgeRequest.Header.Command, addBridgeRequest.Header.Seq, Errors.BRIDGE_EXISTS);
            }

            try
            {
                using (var runtimeClient = new RuntimeClient(addBridgeRequest.TargetUrn, addBridgeRequest.TargetPort))
                {
                    await runtimeClient.HeartBeat();
                }
            }
            catch (RuntimeClientException)
            {
                throw new RuntimeException(addBridgeRequest.Header.Command, addBridgeRequest.Header.Seq, Errors.INVALID_BRIDGE);
            }
        }
    }
}
