using FaasNet.CLI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CLI.Commands
{
    public class FunctionConfigurationCommand : IMenuItemCommand
    {
        public string Command => "configuration";
        public string Description => "Invoke a function";

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
                Console.WriteLine($"The configuration file '{ConfigurationHelper.ConfigurationFileName}' doesn't exist");
                return;
            }

            var name = args.First();
            var gatewayClient = new GatewayClient();
            var jObj = gatewayClient.GetConfiguration(configuration.Provider.Gateway, name);
            Console.WriteLine(jObj.ToString());
        }
    }
}
