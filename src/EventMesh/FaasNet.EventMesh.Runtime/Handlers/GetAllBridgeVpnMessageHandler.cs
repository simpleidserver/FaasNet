using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class GetAllBridgeVpnMessageHandler : IMessageHandler
    {
        private readonly IBridgeServerStore _bridgeServerStore;

        public GetAllBridgeVpnMessageHandler(IBridgeServerStore bridgeServerStore)
        {
            _bridgeServerStore = bridgeServerStore;
        }

        public Commands Command => Commands.GET_ALL_BRIDGE_VPN_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var bridgeServerLst = await _bridgeServerStore.GetAll(cancellationToken);
            return EventMeshPackageResult.SendResult(PackageResponseBuilder.GetBridges(bridgeServerLst.Select(b => new BridgeServerResponse
            {
                SourceVpn = b.SourceVpn,
                TargetPort = b.TargetPort,
                TargetUrn = b.TargetUrn,
                TargetVpn = b.TargetVpn
            }).ToList(), package.Header.Seq));
        }
    }
}
