using EventMesh.Runtime.Messages;
using System;

namespace EventMesh.Runtime.Exceptions
{
    public class RuntimeException : Exception
    {
        public RuntimeException(Commands sourceCommand, string sourceSeq, Errors error)
        {
            SourceCommand = sourceCommand;
            SourceSeq = sourceSeq;
            Error = error;
        }

        public Commands SourceCommand { get; set; }
        public string SourceSeq { get; set; }
        public Errors Error { get; set; }
    }
}
