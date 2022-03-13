using FaasNet.CLI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CLI.Commands
{
    public class ConfigurationGetCommand : IMenuItemCommand
    {
        public string Command => "-get";
        public string Description => "Get the configuration";

        public void Execute(IEnumerable<string> args)
        {
            if (!args.Any())
            {
                Console.WriteLine("A Property with its value must be specified");
                return;
            }

            var configuration = ConfigurationHelper.GetConfiguration();
            if (configuration == null)
            {
                Console.WriteLine($"The configuration file '{ConfigurationHelper.ConfigurationFileName}' doesn't exist");
                return;
            }

            var key = args.First();
            if (!ConfigurationHelper.HasKey(key))
            {
                Console.WriteLine($"The key '{key}' cannot be configured");
                return;
            }

            if(key == ConfigurationHelper.GatewayKey)
            {
                Console.WriteLine($"{key}={configuration.Provider.Gateway}");
            }
        }
    }
}
