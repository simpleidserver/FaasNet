using FaasNet.Common.Helpers;
using FaasNet.Peer.Client.Transports;
using System;
using System.Net;

namespace FaasNet.Peer.Client
{
    public interface IPeerClientFactory
    {
        T Build<T>(string url, int port) where T : BasePeerClient;
        T Build<T>(IPEndPoint edp) where T : BasePeerClient;
    }

    public class PeerClientFactory : IPeerClientFactory
    {
        private readonly IClientTransportFactory _transportFactory;

        public PeerClientFactory(IClientTransportFactory transportFactory)
        {
            _transportFactory = transportFactory;
        }

        public T Build<T>(string url, int port) where T : BasePeerClient
        {
            return Build<T>(new IPEndPoint(DnsHelper.ResolveIPV4(url), port));
        }

        public T Build<T>(IPEndPoint edp) where T : BasePeerClient
        {
            var transport = _transportFactory.Create();
            var instance = Activator.CreateInstance(typeof(T), transport);
            ((BasePeerClient)instance).Open(edp);
            return (T)instance;
        }

        public static T Build<T>(IPEndPoint edp, IClientTransport clientTransport) where T : BasePeerClient
        {
            var instance = Activator.CreateInstance(typeof(T), clientTransport);
            ((BasePeerClient)instance).Open(edp);
            return (T)instance;
        }

        public static T Build<T>(string url, int port, IClientTransport clientTransport) where T : BasePeerClient
        {
            var instance = Activator.CreateInstance(typeof(T), clientTransport);
            ((BasePeerClient)instance).Open(url, port);
            return (T)instance;
        }
    }
}
