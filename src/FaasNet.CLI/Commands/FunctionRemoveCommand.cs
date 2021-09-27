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
                Console.WriteLine("The name must be specified");
                return;
            }

            var configuration = ConfigurationHelper.GetConfiguration();
            if (configuration == null)
            {
                Console.WriteLine($"The configuration file '{ConfigurationHelper.GatewayKey}' doesn't exist");
                return;
            }

            var name = args.First();
            var client = new GatewayClient();
            client.UnpublishFunction(configuration.Provider.Gateway, name);
            var fn = configuration.Functions.First(f => f.Name == name);
            configuration.Functions.Remove(fn);
            ConfigurationHelper.UpdateConfiguration(configuration);
            Console.WriteLine($"The function '{name}' is remove");
        }
    }
}
