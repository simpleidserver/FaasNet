namespace FaasNet.RaftConsensus.Core.StateMachines
{
    public interface ICommand
    {
        string StateMachineId { get; set; }
    }
}
