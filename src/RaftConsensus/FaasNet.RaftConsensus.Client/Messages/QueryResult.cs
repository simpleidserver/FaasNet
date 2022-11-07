using FaasNet.Peer.Client;
using System;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class QueryResult : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.QUERY_RESULT;

        public bool IsFound { get; set; }
        public IQueryResult Result { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteBoolean(IsFound);
            if (!IsFound) return;
            context.WriteString(Result.GetType().AssemblyQualifiedName);
            Result.Serialize(context);
        }

        public QueryResult Extract(ReadBufferContext context)
        {
            IsFound = context.NextBoolean();
            if (!IsFound) return this;
            var result = (IQueryResult)Activator.CreateInstance(Type.GetType(context.NextString()));
            result.Deserialize(context);
            Result = result;
            return this;
        }
    }
}
