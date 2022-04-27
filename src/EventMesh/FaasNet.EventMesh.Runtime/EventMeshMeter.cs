using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace FaasNet.EventMesh.Runtime
{
    public static class EventMeshMeter
    {
        private static Meter _eventMeshMeter = new (MeterName, Version);
        private static object _nbIncomingRequestLock = new object();
        private static object _nbOutgoingRequestLock = new object();
        private static int _nbIncomingRequest = 0;
        private static int _nbOutgoingRequest = 0;

        public static string MeterName => "EventMeshMeter";
        public static string ActivitySourceName => "EventMeshActivitySource";
        public static string Version => "1.0";

        public static ActivitySource RequestActivitySource = new ActivitySource(ActivitySourceName);

        public static void IncrementNbIncomingRequest()
        {
            lock(_nbIncomingRequestLock)
            {
                _nbIncomingRequest++;
            }
        }

        public static void IncrementNbOutgoingRequest()
        {
            lock(_nbOutgoingRequestLock)
            {
                _nbOutgoingRequest++;
            }
        }

        public static void Init()
        {
            _eventMeshMeter.CreateObservableCounter<int>("incomingRequest", () =>
            {
                lock(_nbIncomingRequestLock)
                {
                    var result = _nbIncomingRequest;
                    _nbIncomingRequest = 0;
                    return result;
                }
            }, "nb", "Number of incoming requests");
            _eventMeshMeter.CreateObservableCounter<int>("outgoingRequest", () =>
            {
                lock(_nbOutgoingRequestLock)
                {
                    var result = _nbOutgoingRequest;
                    _nbOutgoingRequest = 0;
                    return result;
                }
            }, "nb", "Number of outgoing requests");
        }
    }
}
