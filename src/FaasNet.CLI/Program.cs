using FaasNet.CLI.Commands;
using FaasNet.CLI.Helpers;
using System.Collections.Generic;
using System.Diagnostics;

namespace FaasNet.CLI
{
    class Program
    {
        private static List<IMenuItemCommand> Commands = new List<IMenuItemCommand>
        {
            new ApplyCommand(),
            new FunctionCommand()
        };

        static void Main(string[] args)
        {
            MenuHelper.Execute(args, Commands);
        }
    }
}
