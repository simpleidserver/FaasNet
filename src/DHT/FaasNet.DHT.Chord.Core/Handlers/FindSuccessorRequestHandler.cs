using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class FindSuccessorRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;

        public FindSuccessorRequestHandler(IDHTPeerInfoStore peerInfoStore)
        {
            _peerInfoStore = peerInfoStore;
        }

        public Commands Command => Commands.FIND_SUCCESSOR_REQUEST;

        public Task<DHTPackage> Handle(DHTPackage request, CancellationToken token)
        {
            var findSuccessorRequest = request as FindSuccessorRequest;
            var successor = FindSuccessor(findSuccessorRequest.NodeId);
            DHTPackage result = PackageResponseBuilder.FindSuccessor(successor.Url, successor.Port, successor.Id);
            return Task.FromResult(result);
        }

        private PeerInfoSuccessor FindSuccessor(long id)
        {
            var peerInfo = _peerInfoStore.Get();
            foreach (var successor in peerInfo.Successors)
            {
                if (CheckIntervalEquivalence(peerInfo.Id, id, successor.Id, peerInfo.DimensionFingerTable)) return successor;
            }

            return ClosestPrecedingPeer(id);
        }

        private PeerInfoSuccessor ClosestPrecedingPeer(long id)
        {
            var peerInfo = _peerInfoStore.Get();
            for (int i = peerInfo.Successors.Count() - 1; i >= 0; i--)
            {
                var nodeIndex = peerInfo.Successors.ElementAt(i).Id;
                if(CheckIntervalClosest(nodeIndex, id, peerInfo.Id, peerInfo.DimensionSuccessor))
                {
                    return peerInfo.Successors.ElementAt(i);
                }
            }

            return null;
        }

        private bool CheckIntervalEquivalence(long pred, long index, long succ, int m)
        {
            if (pred == succ) return true;
            if (pred > succ) return (index > pred && index < Math.Pow(m, 2)) || (index >= 0 && index <= succ);
            else return index > pred && index <= succ;
        }

        private bool CheckIntervalClosest(long pred, long index, long succ, int dimensionSuccessor)
        {
            if (pred == succ) return false;
            if (pred > succ) return (index > pred && index < Math.Pow(dimensionSuccessor, 2)) || (index >= 0 && index < succ);
            else return index > pred && index < succ;
        }
    }
}
