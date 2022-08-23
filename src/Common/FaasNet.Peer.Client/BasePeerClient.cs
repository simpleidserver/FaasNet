using FaasNet.Common.Helpers;
using System.Net;

namespace FaasNet.Peer.Client
{
    public abstract class BasePeerClient : IPeerClient
    {
        public BasePeerClient(IPEndPoint target)
        {
            Target = target;
        }

        public BasePeerClient(string url, int port) : this(new IPEndPoint(DnsHelper.ResolveIPV4(url), port)) { }

        protected IPEndPoint Target { get; set; }

        public abstract void Dispose();
    }
}
