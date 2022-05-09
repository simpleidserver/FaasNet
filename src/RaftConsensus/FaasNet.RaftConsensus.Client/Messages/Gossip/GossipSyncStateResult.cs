using System.Collections.Generic;

namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public class GossipSyncStateResult : GossipPackage
    {
        public GossipSyncStateResult()
        {
            States = new List<GossipStateResult>();
        }

        public ICollection<GossipStateResult> States { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteInteger(States.Count);
            foreach (var state in States) state.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var length = context.NextInt();
            for (int i = 0; i < length; i++) States.Add(GossipStateResult.Deserialize(context));
        }
    }

    public class GossipStateResult
    {
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public int EntityVersion { get; set; }
        public string Value { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(EntityType);
            context.WriteString(EntityId);
            context.WriteInteger(EntityVersion);
            context.WriteString(Value);
        }

        public static GossipStateResult Deserialize(ReadBufferContext context)
        {
            return new GossipStateResult { EntityType = context.NextString(), EntityId = context.NextString(), EntityVersion = context.NextInt(), Value = context.NextString() };
        }
    }
}
