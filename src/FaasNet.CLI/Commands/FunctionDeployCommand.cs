using FaasNet.CLI.Helpers;
using FaasNet.CLI.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CLI.Commands
{
    public class FunctionDeployCommand : IMenuItemCommand
    {
        public string Command => "deploy";
        public string Description => "Deploy the function into a local or remote Gateway";

        public void Execute(IEnumerable<string> args)
        {
            if (!args.Any() || args.Count() != 6)
            {
                Console.WriteLine("The -name, -image and -version arguments must be specified");
                return;
            }

            var configuration = ConfigurationHelper.GetConfiguration();
            if (configuration == null)
            {
                Console.WriteLine($"The configuration file '{ConfigurationHelper.ConfigurationFileName}' doesn't exist");
                return;
            }

            string errorMessage;
            DeployFunctionParameter parameter;
            if (!InputParameterParser.TryParse(args, out errorMessage, out parameter))
            {
                Console.WriteLine(errorMessage);
                return;
            }

            var client = new GatewayClient();
            var publishFunctionResult = client.PublishFunction(configuration.Provider.Gateway, parameter.Name, parameter.Image, parameter.Version);
            Console.WriteLine($"New id: '{publishFunctionResult.Id}'");
        }
    }
}
