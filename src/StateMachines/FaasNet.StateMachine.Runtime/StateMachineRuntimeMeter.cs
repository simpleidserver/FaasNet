using System.Diagnostics.Metrics;

namespace FaasNet.StateMachine.Runtime
{
    public static class StateMachineRuntimeMeter
    {
        private static Meter _stateMachineMeter;
        private static Counter<int> _createdStateMachineInstances;
        private static Counter<int> _terminatedStateMachineInstances;
        private static int _activeSubscriptions;
        private static object _lockCreatedStateMachineInstance = new object();
        private static object _lockTerminatedStateMachineInstance = new object();
        private static object _lockActiveSubscriptions = new object();
        private static object _lockIsInitialized = new object();
        private static bool _isInitialized = false;

        public static string Version => "1.0";

        public static void IncrementCreatedStateMachineInstance(string workerName)
        {
            lock(_lockCreatedStateMachineInstance)
            {
                TryInit(workerName);
                _createdStateMachineInstances.Add(1);
            }
        }

        public static void IncrementTerminatedStateMachineInstance(string workerName)
        {
            lock(_lockTerminatedStateMachineInstance)
            {
                TryInit(workerName);
                _terminatedStateMachineInstances.Add(1);
            }
        }

        public static void SetActiveSubscriptions(int activeSubscriptions, string workerName)
        {
            lock(_lockActiveSubscriptions)
            {
                TryInit(workerName);
                _activeSubscriptions = activeSubscriptions;
            }
        }

        private static bool TryInit(string workerName)
        {
            lock(_lockIsInitialized)
            {
                if (_isInitialized)
                {
                    return false;
                }

                _stateMachineMeter = new(workerName, Version);
                _createdStateMachineInstances = _stateMachineMeter.CreateCounter<int>("CreatedStateMachineInstances", "nb", "Created state machine instances");
                _terminatedStateMachineInstances = _stateMachineMeter.CreateCounter<int>("TerminatedStateMachineInstances", "nb", "Terminated state machine instances");
                _stateMachineMeter.CreateObservableGauge<int>("ActiveSubscriptions", () => _activeSubscriptions, "nb", "Active subscriptions");
                _isInitialized = true;
                return true;
            }
        }
    }
}
