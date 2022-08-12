using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class AppendEntriesRequest : BaseConsensusPackage
    {
        public AppendEntriesRequest()
        {
            Entries = new List<LogEntry>();
        }

        public override ConsensusCommands Command => ConsensusCommands.APPEND_ENTRIES_REQUEST;

        /// <summary>
        /// Leader's term.
        /// </summary>
        public long Term { get; set; }
        /// <summary>
        /// So follower can redirect clients.
        /// </summary>
        public string LeaderId { get; set; }
        /// <summary>
        /// Index of log entry immediately preceding new ones.
        /// </summary>
        public long PrevLogIndex { get; set; }
        /// <summary>
        /// Term of prevLogIndex entry.
        /// </summary>
        public long PreLogTerm { get; set; }
        /// <summary>
        /// Log entries to store (empty for heartbeat, may send more than one for efficiency).
        /// </summary>
        public IEnumerable<LogEntry> Entries { get; set; }
        /// <summary>
        /// Leader's commitIndex.
        /// </summary>
        public long LeaderCommit { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Term);
            context.WriteString(LeaderId);
            context.WriteLong(PrevLogIndex);
            context.WriteLong(PreLogTerm);
            context.WriteInteger(Entries.Count());
            foreach (var entry in Entries) entry.Serialize(context);
            context.WriteLong(LeaderCommit);
        }

        public void Extract(ReadBufferContext context)
        {
            Term = context.NextLong();
            LeaderId = context.NextString();
            PrevLogIndex = context.NextLong();
            PreLogTerm = context.NextLong();
            var nbEntries = context.NextInt();
            var result = new List<LogEntry>();
            for(var i = 0; i < nbEntries; i++) result.Add(LogEntry.Deserialize(context));
            Entries = result;
            LeaderCommit = context.NextLong();
        }
    }
}
