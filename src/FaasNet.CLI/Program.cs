using FaasNet.CLI.Commands;
using FaasNet.CLI.Helpers;
using System.Collections.Generic;

namespace FaasNet.CLI
{
    class Program
    {
        private static List<IMenuItemCommand> Commands = new List<IMenuItemCommand>
        {
            new ApplyCommand()
        };

        static void Main(string[] args)
        {
            MenuHelper.Execute(args, Commands);
        }
    }
}
