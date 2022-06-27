using System.Collections.Generic;

namespace FaasNet.DHT.Chord.Core
{
    public class DHTPeerInfo
    {
        public DHTPeerInfo(string url, int port, int dimensionFingerTable, int dimensionSuccessor)
        {
            Id = HashHelper.ComputePeerId(url, port, dimensionFingerTable);
            DimensionFingerTable = dimensionFingerTable;
            DimensionSuccessor = dimensionSuccessor;
            Successors = new List<PeerInfoSuccessor>
            {
                new PeerInfoSuccessor { Id = Id, Port = port, Url = url }
            };
        }

        public long Id { get; private set; }
        public int DimensionFingerTable { get; private set; }
        public int DimensionSuccessor { get; private set; }
        public ICollection<PeerInfoSuccessor> Successors { get; set; }
    }

    public class PeerInfoSuccessor
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }
}
