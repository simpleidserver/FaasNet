using FaasNet.CLI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CLI.Helpers
{
    public static class MenuHelper
    {
        public static void Execute(IEnumerable<string> args, List<IMenuItemCommand> commands)
        {
            if (!args.Any())
            {
                DisplayHelper(commands);
                return;
            }

            var cmd = commands.FirstOrDefault(c => c.Command == args.First());
            if (cmd == null)
            {
                DisplayHelper(commands);
                return;
            }

            cmd.Execute(args.Skip(1));
        }

        private static void DisplayHelper(List<IMenuItemCommand> commands)
        {
            foreach (var cmd in commands)
            {
                Console.WriteLine($"{cmd.Command} \t\t {cmd.Description}");
            }
        }
    }
}
