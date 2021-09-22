using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CLI.Commands
{
    public class ApplyFileCommand : IMenuItemCommand
    {
        public string Command => "-f";

        public string Description => "Apply file configuration";

        public void Execute(IEnumerable<string> args)
        {
            if (!args.Any())
            {
                Console.WriteLine("A file must be specified");
                return;
            }

            ApplyCommand.Apply(args.First());
        }
    }
}
