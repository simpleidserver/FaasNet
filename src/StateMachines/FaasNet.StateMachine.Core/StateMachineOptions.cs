namespace FaasNet.StateMachine.Core
{
    public class StateMachineOptions
    {
        public StateMachineOptions()
        {
            StateMachineWorkerUrl = "http://localhost:5010";
        }

        public string FunctionApiBaseUrl { get; set; }
        public string StateMachineWorkerUrl { get; set; }
    }
}
