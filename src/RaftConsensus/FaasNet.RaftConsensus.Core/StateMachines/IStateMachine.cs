using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.RaftConsensus.Core.StateMachines
{
    public interface IStateMachine
    {
        void Apply(ICommand cmd);
        byte[] Serialize();
        void Deserialize(ReadBufferContext context);
    }
}
