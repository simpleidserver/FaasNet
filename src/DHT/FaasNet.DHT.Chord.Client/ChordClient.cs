using FaasNet.DHT.Chord.Client.Extensions;
using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Client
{
    public class ChordClient : BasePeerClient
    {
        public ChordClient(IClientTransport transport) : base(transport) { }

        public async Task Create(int dimFingerTable, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = PackageRequestBuilder.Create(dimFingerTable);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            await Send(writeBufferContext.Buffer.ToArray(), timeoutMS, cancellationToken);
            await Receive(timeoutMS, cancellationToken);
        }

        public async Task Join(string url, int port, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = PackageRequestBuilder.Join(url, port);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            await Send(writeBufferContext.Buffer.ToArray(), timeoutMS, cancellationToken);
            await Receive(timeoutMS, cancellationToken);
        }

        public async Task<int> GetDimensionFingerTable(int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = PackageRequestBuilder.GetDimensionFingerTable();
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            await Send(writeBufferContext.Buffer.ToArray(), timeoutMS, cancellationToken);
            var payload = await Receive(timeoutMS, cancellationToken);
            var readBufferContext = new ReadBufferContext(payload);
            var result = ChordPackage.Deserialize(readBufferContext) as GetDimensionFingerTableResult;
            return result.Dimension;
        }

        public async Task<FindSuccessorResult> FindSuccessor(long nodeId, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = PackageRequestBuilder.FindSuccessor(nodeId);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            await Send(writeBufferContext.Buffer.ToArray(), timeoutMS, cancellationToken);
            var payload = await Receive(timeoutMS, cancellationToken);
            var readBufferContext = new ReadBufferContext(payload);
            var result = ChordPackage.Deserialize(readBufferContext) as FindSuccessorResult;
            return result;
        }

        public async Task<FindPredecessorResult> FindPredecessor(int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = PackageRequestBuilder.FindPredecessor();
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            await Send(writeBufferContext.Buffer.ToArray(), timeoutMS);
            var payload = await Receive(timeoutMS, cancellationToken);
            var readBufferContext = new ReadBufferContext(payload);
            var result = ChordPackage.Deserialize(readBufferContext) as FindPredecessorResult;
            return result;
        }

        public async Task Notify(string url, int port, long id, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = PackageRequestBuilder.Notify(url, port, id);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            await Send(writeBufferContext.Buffer.ToArray(), timeoutMS);
            await Receive(timeoutMS, cancellationToken);
        }

        public async Task<string> GetKey(long id, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = PackageRequestBuilder.GetKey(id);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            await Send(writeBufferContext.Buffer.ToArray(), timeoutMS, cancellationToken);
            var payload = await Receive(timeoutMS, cancellationToken);
            var readBufferContext = new ReadBufferContext(payload);
            var result = ChordPackage.Deserialize(readBufferContext) as GetKeyResult;
            return result.Value;
        }

        public async Task AddKey(long id, string value, bool force = false, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = PackageRequestBuilder.AddKey(id, value, force);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            await Send(writeBufferContext.Buffer.ToArray(), timeoutMS, cancellationToken);
            await Receive(timeoutMS, cancellationToken);
        }
    }
}
