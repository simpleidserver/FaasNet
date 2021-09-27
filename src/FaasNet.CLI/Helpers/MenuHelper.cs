using FaasNet.CLI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CLI.Helpers
{
    public static class MenuHelper
    {
        private static int Padding = 10;

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
            var maxWidth = GetMaxWidth(commands.Select(c => c.Command));
            foreach (var cmd in commands)
            {
                var space = string.Join(string.Empty, Enumerable.Repeat(" ", maxWidth - cmd.Command.Length + Padding));
                Console.WriteLine($"{cmd.Command} {space} {cmd.Description}");
            }
        }

        private static int GetMaxWidth(IEnumerable<string> str)
        {
            return str.Select(r => r.Length).Max();
        }
    }
}
