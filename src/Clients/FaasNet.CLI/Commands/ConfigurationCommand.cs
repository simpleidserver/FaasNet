using FaasNet.CLI.Helpers;
using System.Collections.Generic;

namespace FaasNet.CLI.Commands
{
    public class ConfigurationCommand : IMenuItemCommand
    {
        private List<IMenuItemCommand> _commands = new List<IMenuItemCommand>
        {
            new ConfigurationUpdateCommand(),
            new ConfigurationGetCommand()
        };

        public string Command => "configuration";
        public string Description => "Manage configuration";

        public void Execute(IEnumerable<string> args)
        {
            MenuHelper.Execute(args, _commands);
        }
    }
}
