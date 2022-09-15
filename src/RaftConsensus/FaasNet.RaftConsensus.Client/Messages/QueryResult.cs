using FaasNet.Peer.Client;
using System;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class QueryResult : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.QUERY_RESULT;

        public IQueryResult Result { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Result.GetType().AssemblyQualifiedName);
            Result.Serialize(context);
        }

        public QueryResult Extract(ReadBufferContext context)
        {
            var result = (IQueryResult)Activator.CreateInstance(Type.GetType(context.NextString()));
            result.Deserialize(context);
            Result = result;
            return this;
        }
    }
}
