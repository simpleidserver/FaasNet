using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.DHT.Kademlia.Core
{
    public class DHTPeerInfo
    {
        private DHTPeerInfo(long id, string url, int port, List<KBucket> kbucketLst)
        {
            Id = id;
            Url = url;
            Port = port;
            KBucketLst = kbucketLst;
        }

        public long Id { get; private set; }
        public string Url { get; set; }
        public int Port { get; set; }
        public ICollection<KBucket> KBucketLst { get; private set; }

        public static DHTPeerInfo Create(long id, string url, int port, int nbBits)
        {
            var kbucketLst = new List<KBucket>();
            for (var i = 0; i < nbBits; i++)
            {
                var start = Math.Pow(2, nbBits - i - 1);
                var end = Math.Pow(2, nbBits - i) - 1;
                kbucketLst.Add(new KBucket(start, end));
            }

            return new DHTPeerInfo(id, url, port, kbucketLst);
        }

        public bool TryAddPeer(string url, int port, long peerId)
        {
            // https://kelseyc18.github.io/kademlia_vis/basics/3/
            // https://docs.rs/kademlia_routing_table/latest/kademlia_routing_table/
            var distance = Id ^ peerId;
            distance = distance == 0 ? 1 : distance;
            var bucket = FindClosestKBucket(distance);
            if (bucket == null) return false;
            var record = bucket.Peers.FirstOrDefault(p => p.Url == url && p.Port == port);
            if (record != null)
            {
                record.LastSeenDateTime = DateTime.UtcNow;
                return false;
            }

            bucket.Peers.Add(new KBucketPeer { PeerId = peerId, Url = url, Port = port, LastSeenDateTime = DateTime.UtcNow });
            return true;
        }

        public IEnumerable<KBucketPeer> FindClosestPeers(long key, int k)
        {
            var peers = KBucketLst
                .SelectMany(k => k.Peers)
                .OrderBy(p => p.ComputeDistance(key))
                .Take(k);
            return peers;
        }

        private KBucket FindClosestKBucket(long key)
        {
            return KBucketLst.FirstOrDefault(b => b.Start <= key && key <= b.End);
        }
    }

    public class KBucket
    {
        public KBucket(double start, double end)
        {
            Start = start;
            End = end;
            Peers = new List<KBucketPeer>();
        }

        public double Start { get; set; }
        public double End { get; set; }
        public ICollection<KBucketPeer> Peers { get; set; }

        public KBucketPeer GetClosestPeer(long key)
        {
            KBucketPeer result = null;
            long? distance = null;
            foreach(var peer in Peers)
            {
                var newDistance = peer.ComputeDistance(key);
                if (distance != null) distance = newDistance;
                else if(newDistance < distance)
                {
                    distance = newDistance;
                    result = peer;
                }
            }

            return result;
        }
    }

    public class KBucketPeer
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public long PeerId { get; set; }
        public DateTime LastSeenDateTime { get; set; }

        public long ComputeDistance(long id)
        {
            return PeerId ^ id;
        }
    }
}
