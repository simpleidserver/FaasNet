using System;

namespace FaasNet.DHT.Chord.Core
{
    public static class IntervalHelper
    {
        public static bool CheckInterval(DHTPeerInfo peerInfo, long index)
        {
            var pred = peerInfo.PredecessorPeer.Id;
            var succ = peerInfo.Peer.Id;
            return CheckInterval(pred, index, succ, peerInfo.DimensionFingerTable);
        }

        public static bool CheckIntervalEquivalence(long pred, long index, long succ, int dimensionFingerTable)
        {
            if (pred == succ) return true;
            if (pred > succ) return (index > pred && index < Math.Pow(2, dimensionFingerTable)) || (index >= 0 && index <= succ);
            else return index > pred && index <= succ;
        }

        public static bool CheckInterval(long pred, long index, long succ, int dimensionFingerTable)
        {
            if (pred == succ) return true;
            if (pred > succ) return (index > pred && index < Math.Pow(2, dimensionFingerTable)) || (index >= 0 && index < succ);
            else return index > pred && index < succ;
        }

        public static bool CheckIntervalClosest(long pred, long index, long succ, int dimensionFingerTable)
        {
            if (pred == succ) return false;
            if (pred > succ) return (index > pred && index < Math.Pow(2, dimensionFingerTable)) || (index >= 0 && index < succ);
            else return index > pred && index < succ;
        }
    }
}
