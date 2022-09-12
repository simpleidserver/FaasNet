using System.Collections.Generic;

namespace FaasNet.Peer.Client.Messages
{
    public class GetAllNodesResult : BasePartitionedRequest
    {
        public GetAllNodesResult()
        {
            Nodes = new List<NodeResult>();
        }

        public override PartitionedCommands Command => PartitionedCommands.GET_ALL_NODES_RESULT;

        public ICollection<NodeResult> Nodes { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(Nodes.Count);
            foreach (var node in Nodes) node.Serialize(context);
        }

        public GetAllNodesResult Extract(ReadBufferContext context)
        {
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++) Nodes.Add(NodeResult.Deserialize(context));
            return this;
        }
    }

    public class NodeResult
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public string Id { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Url);
            context.WriteInteger(Port);
            context.WriteString(Id);
        }

        public static NodeResult Deserialize(ReadBufferContext context)
        {
            return new NodeResult
            {
                Url = context.NextString(),
                Port = context.NextInt(),
                Id = context.NextString()
            };
        }
    }
}
