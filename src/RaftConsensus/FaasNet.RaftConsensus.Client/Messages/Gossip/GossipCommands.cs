﻿namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public class GossipCommands: BaseCommands
    {
        public static GossipCommands HEARTBEAT_REQUEST = new GossipCommands(0, "HEARTBEAT_REQUEST");
        public static GossipCommands HEARTBEAT_RESULT = new GossipCommands(1, "HEARTBEAT_RESULT");
        public static GossipCommands SYNC_REQUEST = new GossipCommands(2, "SYNC_REQUEST");
        public static GossipCommands SYNC_RESULT = new GossipCommands(3, "SYNC_RESULT");
        public static GossipCommands UPDATE_NODE_STATE_REQUEST = new GossipCommands(4, "UPDATE_NODE_STATE_REQUEST");
        public static GossipCommands ADD_PEER_REQUEST = new GossipCommands(5, "ADD_PEER_REQUEST");
        public static GossipCommands GET_CLUSTER_NODES_REQUEST = new GossipCommands(6, "GET_CLUSTER_NODES_REQUEST");
        public static GossipCommands GET_CLUSTER_NODES_RESULT = new GossipCommands(7, "GET_CLUSTER_NODES_RESULT");

        protected GossipCommands(int code)
        {
            Init<GossipCommands>(code);
        }

        protected GossipCommands(int code, string name) : base(code, name) { }

        public static GossipCommands Deserialize(ReadBufferContext context)
        {
            return new GossipCommands(context.NextInt());
        }
    }
}
