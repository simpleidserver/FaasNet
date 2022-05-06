using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;

namespace FaasNet.EventMesh.Runtime.Gossip
{
    public class ClientSynchronizer : BaseSynchronizer
    {
        public ClientSynchronizer(IOptions<ConsensusNodeOptions> nodeOptions, IClusterStore clusterStore) : base(nodeOptions, clusterStore) { }

        protected override string EntityType => "Client";
        protected override int EntityVersion => 0;
    }
}