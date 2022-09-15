using FaasNet.Peer.Client;
using System;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class QueryRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.QUERY_REQUEST;

        public IQuery Query { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Query.GetType().AssemblyQualifiedName);
            Query.Serialize(context);
        }

        public QueryRequest Extract(ReadBufferContext context)
        {
            var instance = (IQuery)Activator.CreateInstance(Type.GetType(context.NextString()));
            instance.Deserialize(context);
            Query = instance;
            return this;
        }
    }
}
