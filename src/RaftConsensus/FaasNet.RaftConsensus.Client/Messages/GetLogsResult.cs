using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetLogsResult : BaseConsensusPackage
    {
        public GetLogsResult()
        {
            Entries = new List<LogEntry>();
        }

        public override ConsensusCommands Command => ConsensusCommands.GET_LOGS_RESULT;

        public IEnumerable<LogEntry> Entries { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(Entries.Count());
            foreach (var entry in Entries) entry.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            int nb = context.NextInt();
            var result = new List<LogEntry>();
            for (var i = 0; i < nb; i++) result.Add(LogEntry.Deserialize(context));
            Entries = result;
        }
    }
}
