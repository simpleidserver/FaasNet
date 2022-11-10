using FaasNet.Common.Helpers;
using FaasNet.Peer.Client.Transports;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Client
{
    public abstract class BasePeerClient : IDisposable
    {
        private IClientTransport _transport;

        public BasePeerClient(IClientTransport transport)
        {
            _transport = transport;
        }

        protected IClientTransport Transport => _transport;

        public void Open(string url, int port)
        {
            Open(new IPEndPoint(DnsHelper.ResolveIPV4(url), port));
        }

        public void Open(IPEndPoint edp)
        {
            _transport.Open(edp);
        }

        public Task Send(byte[] payload, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _transport.Send(payload, timeoutMS, cancellationToken);
        }

        public Task<byte[]> Receive(int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _transport.Receive(timeoutMS, cancellationToken);
        }

        public void Dispose()
        {
            _transport.Dispose();
        }
    }
}
