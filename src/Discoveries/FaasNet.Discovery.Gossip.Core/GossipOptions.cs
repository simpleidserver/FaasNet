namespace FaasNet.Discovery.Gossip.Core
{
    public class GossipOptions
    {
        public GossipOptions()
        {
            BroadcastRumorToRandomNeighbourTimerMS = 2 * 1000;
            MaxNbPeersToBroadcastMessage = 3;
        }

        /// <summary>
        /// Broadcast rumor to a set of random neighbours.
        /// </summary>
        public int BroadcastRumorToRandomNeighbourTimerMS { get; set; }

        /// <summary>
        /// Maximum number of Peers where the message will be broadcasted.
        /// </summary>
        public int MaxNbPeersToBroadcastMessage { get; set; }
    }
}
