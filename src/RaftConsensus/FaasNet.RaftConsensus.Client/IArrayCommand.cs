namespace FaasNet.RaftConsensus.Client
{
    public interface IArrayCommand : ICommand
    {
        string Id { get; set; }
    }
}
