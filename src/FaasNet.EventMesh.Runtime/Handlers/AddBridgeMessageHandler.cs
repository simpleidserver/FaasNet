using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Messages;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.Options;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class AddBridgeMessageHandler : IMessageHandler
    {
        private readonly IUdpClientServerFactory _udpClientFactory;
        private readonly IBridgeServerStore _bridgeServerStore;
        private readonly RuntimeOptions _options;

        public AddBridgeMessageHandler(
            IUdpClientServerFactory udpClientFactory,
            IBridgeServerStore bridgeServerStore,
            IOptions<RuntimeOptions> options)
        {
            _udpClientFactory = udpClientFactory;
            _bridgeServerStore = bridgeServerStore;
            _options = options.Value;
        }

        public Commands Command => Commands.ADD_BRIDGE_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var addBridgeRequest = package as AddBridgeRequest;
            var bridgeServer = _bridgeServerStore.Get(addBridgeRequest.Urn);
            if (bridgeServer != null)
            {
                throw new RuntimeException(addBridgeRequest.Header.Command, addBridgeRequest.Header.Seq, Errors.BRIDGE_EXISTS);
            }

            var udpClient = _udpClientFactory.Build();
            try
            {
                var runtimeClient = new RuntimeClient(udpClient, addBridgeRequest.Urn, addBridgeRequest.Port);
                await runtimeClient.HeartBeat();
            }
            catch(RuntimeClientException)
            {
                throw new RuntimeException(addBridgeRequest.Header.Command, addBridgeRequest.Header.Seq, Errors.INVALID_BRIDGE);
            }

            bridgeServer = BridgeServer.Create(addBridgeRequest.Urn, addBridgeRequest.Port);
            _bridgeServerStore.Add(bridgeServer);
            return PackageResponseBuilder.AddBridge(package.Header.Seq);
        }
    }
}
