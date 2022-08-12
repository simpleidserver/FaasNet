using FaasNet.Common.Helpers;
using System;
using System.Net;
using System.Net.Sockets;

namespace FaasNet.RaftConsensus.Client
{
    public class UDPRaftConsensusClient: IDisposable
    {
        private readonly IPEndPoint _target;

        public UDPRaftConsensusClient(IPEndPoint target)
        {
            _target = target;
            UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public UDPRaftConsensusClient(string url, int port) : this(new IPEndPoint(DnsHelper.ResolveIPV4(url), port)) { }

        public UdpClient UdpClient { get; private set; }

        public void Dispose()
        {
            UdpClient?.Dispose();
        }
    }
}
