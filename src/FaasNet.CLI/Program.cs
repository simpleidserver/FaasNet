using FaasNet.CLI.Commands;
using FaasNet.CLI.Helpers;
using System;
using System.Collections.Generic;

namespace FaasNet.CLI
{
    class Program
    {
        private static List<IMenuItemCommand> Commands = new List<IMenuItemCommand>
        {
            new ApplyCommand(),
            new FunctionCommand(),
            new ConfigurationCommand()
        };

        static void Main(string[] args)
        {
            MenuHelper.Execute(args, Commands);
            string ss = "";
        }
    }
}
