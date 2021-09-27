using FaasNet.CLI.Helpers;
using FaasNet.CLI.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CLI.Commands
{
    public class FunctionInvokeCommand : IMenuItemCommand
    {
        public string Command => "invoke";
        public string Description => "Invoke a function";

        public void Execute(IEnumerable<string> args)
        {
            if (!args.Any() || args.Count() != 6)
            {
                Console.WriteLine("The -name, -configuration and -input parameters must be specified");
                return;
            }

            var configuration = ConfigurationHelper.GetConfiguration();
            if (configuration == null)
            {
                Console.WriteLine($"The configuration file '{ConfigurationHelper.ConfigurationFileName}' doesn't exist");
                return;
            }

            InvokeFunctionParameter parameter;
            string errorMessage;
            if (!InputParameterParser.TryParse(args, out errorMessage, out parameter))
            {
                Console.WriteLine(errorMessage);
                return;
            }

            var gatewayClient = new GatewayClient();
            var jObj = gatewayClient.InvokeFunction(configuration.Provider.Gateway, parameter.Name, parameter.Configuration, parameter.Input);
            Console.WriteLine(jObj.ToString());
        }
    }
}
