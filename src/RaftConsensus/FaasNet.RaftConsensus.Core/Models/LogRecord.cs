using System;
using System.Diagnostics;

namespace FaasNet.RaftConsensus.Core.Models
{
    [DebuggerDisplay("Value = {Value}, Index = {Index}")]
    public class LogRecord
    {
        public long Index { get; set; }
        public string Value { get; set; }
        public DateTime InsertionDateTime { get; set; }
    }
}
