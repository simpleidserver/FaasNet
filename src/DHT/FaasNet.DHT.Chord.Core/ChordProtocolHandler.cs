using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Handlers;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core
{
    public class ChordProtocolHandler : IProtocolHandler
    {
        private readonly IEnumerable<IRequestHandler> _requestHandlers;

        public ChordProtocolHandler(IEnumerable<IRequestHandler> requestHandlers)
        {
            _requestHandlers = requestHandlers;
        }

        public string MagicCode => ChordPackage.MAGIC_CODE;

        public async Task<BasePeerPackage> Handle(byte[] payload, CancellationToken cancellationToken)
        {
            var ctx = new ReadBufferContext(payload);
            var package = ChordPackage.Deserialize(ctx, true);
            var requestHandler = _requestHandlers.First(r => r.Command == package.Command);
            var response = await requestHandler.Handle(package, cancellationToken);
            return response;
        }
    }
}
