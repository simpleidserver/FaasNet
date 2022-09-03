using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.StateMachines
{
    public interface IStateMachine
    {
        string Id { get; set; }
        void Apply(ICommand cmd);
        byte[] Serialize();
        void Deserialize(ReadBufferContext context);
    }
}