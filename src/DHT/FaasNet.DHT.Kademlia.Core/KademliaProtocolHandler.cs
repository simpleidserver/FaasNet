using FaasNet.DHT.Kademlia.Client.Messages;
using FaasNet.DHT.Kademlia.Core.Handlers;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core
{
    public class KademliaProtocolHandler : IProtocolHandler
    {
        private readonly IEnumerable<IRequestHandler> _requestHandlers;
        private readonly ILogger<KademliaProtocolHandler> _logger;

        public KademliaProtocolHandler(IEnumerable<IRequestHandler> requestHandlers, ILogger<KademliaProtocolHandler> logger)
        {
            _requestHandlers = requestHandlers;
            _logger = logger;
        }

        public string MagicCode => KademliaPackage.MAGIC_CODE;

        public async Task<BasePeerPackage> Handle(byte[] payload, CancellationToken cancellationToken)
        {
            var bufferContext = new ReadBufferContext(payload);
            var package = KademliaPackage.Deserialize(bufferContext, true);
            _logger.LogInformation("Receive the request {Request}", package.Command.Name);
            var requestHandler = _requestHandlers.First(r => r.Command == package.Command);
            var packageResult = await requestHandler.Handle(package, cancellationToken);
            _logger.LogInformation("Send the result {Result}", packageResult.Command.Name);
            return packageResult;
        }
    }
}
