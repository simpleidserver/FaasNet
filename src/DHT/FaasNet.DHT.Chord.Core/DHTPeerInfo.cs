using System.Collections.Generic;
using System.Diagnostics;

namespace FaasNet.DHT.Chord.Core
{
    public class DHTPeerInfo
    {
        public DHTPeerInfo(string url, int port)
        {
            Peer = new PeerInfo(url, port);
            Fingers = new List<FingerTableRecord>();
        }

        public int DimensionFingerTable { get; private set; }
        public PeerInfo PredecessorPeer { get; set; }
        public PeerInfo SuccessorPeer { get; set; }
        public PeerInfo Peer { get; set; }
        public ICollection<FingerTableRecord> Fingers { get; set; }

        public void ComputeId(int dimensionFingerTable)
        {
            Peer.Id = HashHelper.ComputePeerId(Peer.Url, Peer.Port, dimensionFingerTable);
            DimensionFingerTable = dimensionFingerTable;
        }
    }

    [DebuggerDisplay("Start {Start}, End {End}, Peer {Peer.Id}")]
    public class FingerTableRecord
    {
        public PeerInfo Peer { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
    }

    [DebuggerDisplay("Peer {Id}")]
    public class PeerInfo
    {
        public PeerInfo() { }

        public PeerInfo(string url, int port)
        {
            Url = url;
            Port = port;
        }

        public long Id { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }
}
