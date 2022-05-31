using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class AddBridgeVpnMessageHandler : IMessageHandler
    {
        private readonly ILogger<AddBridgeVpnMessageHandler> _logger;
        private readonly IVpnStore _vpnStore;
        private readonly IBridgeServerStore _bridgeServerStore;

        public AddBridgeVpnMessageHandler(ILogger<AddBridgeVpnMessageHandler> logger, IVpnStore vpnStore, IBridgeServerStore bridgeServerStore)
        {
            _logger = logger;
            _vpnStore = vpnStore;
            _bridgeServerStore = bridgeServerStore;
        }

        public Commands Command => Commands.ADD_BRIDGE_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var addBridgeRequest = package as AddBridgeRequest;
            if (!(await IsServerReachable(addBridgeRequest.TargetUrn, addBridgeRequest.TargetPort, cancellationToken))) return EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(Commands.ADD_BRIDGE_REQUEST, addBridgeRequest.Header.Seq, Errors.TARGET_NOT_REACHABLE));
            var allTargetVpns = await GetAllVpns(addBridgeRequest.TargetUrn, addBridgeRequest.TargetPort, cancellationToken);
            if (!allTargetVpns.Contains(addBridgeRequest.TargetVpn)) return EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(Commands.ADD_BRIDGE_REQUEST, addBridgeRequest.Header.Seq, Errors.UNKNOWN_TARGET_VPN));
            var currentVpns = await _vpnStore.GetAll(cancellationToken);
            if (!currentVpns.Any(vpn => vpn.Name == addBridgeRequest.SourceVpn)) return EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(Commands.ADD_BRIDGE_REQUEST, addBridgeRequest.Header.Seq, Errors.UNKNOWN_SOURCE_VPN));
            await _bridgeServerStore.Add(new Models.BridgeServer 
            { 
                SourceVpn = addBridgeRequest.SourceVpn, 
                TargetPort = addBridgeRequest.TargetPort, 
                TargetUrn = addBridgeRequest.TargetUrn, 
                TargetVpn = addBridgeRequest.TargetVpn 
            }, cancellationToken);
            return EventMeshPackageResult.SendResult(PackageResponseBuilder.AddBridge(addBridgeRequest.Header.Seq));
        }

        private async Task<bool> IsServerReachable(string url, int port, CancellationToken cancellationToken)
        {
            try
            {
                var evtMeshClient = new EventMeshClient(url, port);
                await evtMeshClient.Ping(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        private Task<IEnumerable<string>> GetAllVpns(string url, int port, CancellationToken cancellationToken)
        {
            var evtMeshClient = new EventMeshClient(url, port);
            return evtMeshClient.GetAllVpns(cancellationToken);
        }
    }
}
