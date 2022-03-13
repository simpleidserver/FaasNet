using FaasNet.CLI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CLI.Commands
{
    public class FunctionRemoveCommand : IMenuItemCommand
    {
        public string Command => "-r";
        public string Description => "Remove a function from a locale or remote Gateway";

        public void Execute(IEnumerable<string> args)
        {
            if (!args.Any())
            {
                Console.WriteLine("The identifier must be specified");
                return;
            }

            var configuration = ConfigurationHelper.GetConfiguration();
            if (configuration == null)
            {
                Console.WriteLine($"The configuration file '{ConfigurationHelper.GatewayKey}' doesn't exist");
                return;
            }

            var id = args.First();
            var client = new GatewayClient();
            client.UnpublishFunction(configuration.Provider.Gateway, id);
            Console.WriteLine($"The function '{id}' is remove");
        }
    }
}
