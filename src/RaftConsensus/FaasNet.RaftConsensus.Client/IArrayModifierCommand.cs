using System.Collections.Generic;

namespace FaasNet.RaftConsensus.Client
{
    public interface IArrayModifierCommand<T> : ICommand where T : IEntity
    {
        void Apply(ICollection<T> arr);
    }
}
