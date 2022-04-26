namespace FaasNet.StateMachine.Worker
{
    public class StateMachineWorkerOptions
    {
        public static string DefaultName = "StateMachine-Worker1";

        public StateMachineWorkerOptions()
        {
            WorkerName = "StateMachine-Worker1";
        }

        public string WorkerName { get; set; }
    }
}
