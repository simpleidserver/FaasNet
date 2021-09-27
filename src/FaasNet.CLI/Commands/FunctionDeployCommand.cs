using FaasNet.CLI.Helpers;
using FaasNet.CLI.Parameters;
using FaasNet.Common.Configuration;
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
            if (!args.Any() || args.Count() != 4)
            {
                Console.WriteLine("The -name and -image must be specified");
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
            client.PublishFunction(configuration.Provider.Gateway, parameter.Name, parameter.Image);
            configuration.Functions.Add(new FaasFunctionConfiguration
            {
                Image = parameter.Image,
                Name = parameter.Name
            });
            ConfigurationHelper.UpdateConfiguration(configuration);
            Console.WriteLine($"The function '{parameter.Name}' is published");
        }
    }
}
