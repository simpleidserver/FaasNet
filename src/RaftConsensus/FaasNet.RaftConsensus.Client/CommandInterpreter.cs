using System.Collections.Generic;

namespace FaasNet.RaftConsensus.Client
{
    public static class CommandInterpreter
    {
        public static void Interpret<T>(IEnumerable<IArrayModifierCommand<T>> cmds, ICollection<T> entities) where T : IEntity
        {
            foreach (var cmd in cmds) cmd.Apply(entities);
        }
    }
}
