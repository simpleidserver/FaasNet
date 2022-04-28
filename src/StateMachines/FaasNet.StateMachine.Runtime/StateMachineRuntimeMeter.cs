using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace FaasNet.StateMachine.Runtime
{
    public static class StateMachineRuntimeMeter
    {
        private static Counter<int> _createdStateMachineInstances;
        private static Counter<int> _terminatedStateMachineInstances;
        private static Counter<int> _nbActiveSubscriptions;
        private static object _lockCreatedStateMachineInstance = new object();
        private static object _lockTerminatedStateMachineInstance = new object();
        private static object _lockActiveSubscriptions = new object();

        public static string Version => "1.0";
        public static string Name => "StateMachineWorker";
        public static Meter StateMachineMeter = new Meter(Name, Version);
        public static ActivitySource StateMachineWorkerActivitySource = new ActivitySource(Name, Version);

        public static void IncrementCreatedStateMachineInstance()
        {
            lock(_lockCreatedStateMachineInstance)
            {
                _createdStateMachineInstances.Add(1);
            }
        }

        public static void IncrementTerminatedStateMachineInstance()
        {
            lock(_lockTerminatedStateMachineInstance)
            {
                _terminatedStateMachineInstances.Add(1);
            }
        }

        public static void IncrementActiveSubscriptions()
        {
            lock(_lockActiveSubscriptions)
            {
                _nbActiveSubscriptions.Add(1);
            }
        }

        public static void DecrementActiveSubscriptions()
        {
            lock (_lockActiveSubscriptions)
            {
                _nbActiveSubscriptions.Add(-1);
            }
        }

        public static void Init()
        {
            _createdStateMachineInstances = StateMachineMeter.CreateCounter<int>("CreatedStateMachineInstances", "nb", "Created state machine instances");
            _terminatedStateMachineInstances = StateMachineMeter.CreateCounter<int>("TerminatedStateMachineInstances", "nb", "Terminated state machine instances");
            _nbActiveSubscriptions = StateMachineMeter.CreateCounter<int>("ActiveSubscriptions", "nb", "Active subscriptions");
        }
    }
}
