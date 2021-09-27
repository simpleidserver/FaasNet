using FaasNet.CLI.Helpers;
using System.Collections.Generic;

namespace FaasNet.CLI.Commands
{
    public class FunctionCommand : IMenuItemCommand
    {
        private List<IMenuItemCommand> _commands = new List<IMenuItemCommand>
        {
            new FunctionCreateDockerFileCommand(),
            new FunctionBuildDockerFileCommand(),
            new FunctionPublishDockerFileCommand(),
            new FunctionDeployCommand(),
            new FunctionRemoveCommand(),
            new FunctionInvokeCommand(),
            new FunctionConfigurationCommand()
        };

        public string Command => "function";
        public string Description => "Manage a function";

        public void Execute(IEnumerable<string> args)
        {
            MenuHelper.Execute(args, _commands);
        }
    }
}
