using FaasNet.Common.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
                Console.Write("A file must be specified");
                return;
            }

            ApplyCommand.Apply(args.First());
        }
    }
}
