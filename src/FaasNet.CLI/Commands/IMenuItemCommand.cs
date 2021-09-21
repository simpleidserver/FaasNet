using System.Collections.Generic;

namespace FaasNet.CLI.Commands
{
    public interface IMenuItemCommand
    {
        public string Command { get; }
        public string Description { get; }

        void Execute(IEnumerable<string> args);
    }
}
