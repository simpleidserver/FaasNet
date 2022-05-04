using System.Diagnostics;

namespace FaasNet.RaftConsensus.Core.Models
{
    [DebuggerDisplay("TermId = {TermId}, TermIndex = {TermIndex}")]
    public class PeerInfo
    {
        public string TermId { get; set; }
        public long ConfirmedTermIndex { get; set; }
        public long TermIndex { get; set; }

        public void Upgrade()
        {
            ConfirmedTermIndex++;
            TermIndex = ConfirmedTermIndex;
        }

        public void Reset()
        {
            TermIndex = ConfirmedTermIndex;
        }

        public void Increment()
        {
            TermIndex = ConfirmedTermIndex + 1;
        }
    }
}
