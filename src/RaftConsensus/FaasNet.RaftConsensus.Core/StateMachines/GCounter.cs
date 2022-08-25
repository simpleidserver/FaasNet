namespace FaasNet.RaftConsensus.Core.StateMachines
{
    public class GCounter : IStateMachine
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long Value { get; set; }
    }

    public class IncrementGCounterCommand : ICommand
    {
        public string StateMachineId { get; set; }
        public string Name { get; set; }
    }
}
