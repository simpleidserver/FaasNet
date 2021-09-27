using FaasNet.CLI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CLI.Commands
{
    public class ConfigurationUpdateCommand : IMenuItemCommand
    {
        public string Command => "-set";
        public string Description => "Update the configuration";

        public void Execute(IEnumerable<string> args)
        {
            if (!args.Any())
            {
                Console.WriteLine("A Property with its value must be specified");
                return;
            }

            var splitted = args.First().Split('=');
            if (splitted.Count() != 2) 
            {
                Console.WriteLine("The argument is not correct, it must respect the format Key=Value");
                return;
            }

            var configuration = ConfigurationHelper.GetConfiguration();
            if (configuration == null)
            {
                Console.WriteLine($"The configuration file '{ConfigurationHelper.ConfigurationFileName}' doesn't exist");
                return;
            }

            var key = splitted.First();
            var value = splitted.Last();
            if (!ConfigurationHelper.HasKey(key))
            {
                Console.WriteLine($"The key '{key}' cannot be configured");
                return;
            }

            if (key == ConfigurationHelper.GatewayKey)
            {
                configuration.Provider.Gateway = value;
            }

            ConfigurationHelper.UpdateConfiguration(configuration);
        }
    }
}
