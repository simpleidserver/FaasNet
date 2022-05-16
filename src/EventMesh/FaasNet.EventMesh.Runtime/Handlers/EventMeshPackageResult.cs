using FaasNet.EventMesh.Client.Messages;
using FaasNet.RaftConsensus.Core.Models;
using System;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class EventMeshPackageResult
    {
        public Package Package { get; private set; }
        public string Termid { get; private set; }
        public EventMeshPackageResultStatus Status { get; private set; }
        public LogRecord LogRecord { get; private set; }

        public static EventMeshPackageResult SendResult(Package package)
        {
            return new EventMeshPackageResult { Package = package, Status = EventMeshPackageResultStatus.SEND_RESULT };
        }

        public static EventMeshPackageResult AddPeer(string termid, Package package = null, LogRecord logRecord = null)
        {
            var status = EventMeshPackageResultStatus.ADD_PEER;
            if (package != null) status |= EventMeshPackageResultStatus.SEND_RESULT;
            return new EventMeshPackageResult { Termid = termid, Status = status, Package = package, LogRecord = logRecord };
        }
    }

    [Flags]
    public enum EventMeshPackageResultStatus
    {
        SEND_RESULT = 0,
        ADD_PEER = 1
    }
}
