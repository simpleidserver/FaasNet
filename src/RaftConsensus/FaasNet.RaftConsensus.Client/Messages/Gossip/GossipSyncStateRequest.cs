﻿using System.Collections.Generic;

namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public class GossipSyncStateRequest : GossipPackage
    {
        public GossipSyncStateRequest()
        {
            States = new List<GossipState>();
        }

        public ICollection<GossipState> States { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteInteger(States.Count);
            foreach (var state in States) state.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var length = context.NextInt();
            for (int i = 0; i < length; i++) States.Add(GossipState.Deserialize(context));
        }
    }
}
