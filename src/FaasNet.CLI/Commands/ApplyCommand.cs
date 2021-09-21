using FaasNet.CLI.Helpers;
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
    public class ApplyCommand : IMenuItemCommand
    {
        private List<IMenuItemCommand> _commands = new List<IMenuItemCommand>
        {
            new ApplyFileCommand()
        };

        public string Command => "apply";
        public string Description => "Apply configuration";

        public void Execute(IEnumerable<string> args)
        {
            if (!args.Any())
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "faasnet.yml");
                Apply(filePath);
            }

            MenuHelper.Execute(args, _commands);
        }

        internal static void Apply(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"The file '{filePath}' doesn't exist");
                return;
            }

            var yml = File.ReadAllText(filePath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            var faasConfiguration = deserializer.Deserialize<FaasConfiguration>(yml);
            using (var httpClient = new HttpClient())
            {
                var jObj = new JObject
                {
                    { "SerializedConfigurationFile", Convert.ToBase64String(Encoding.UTF8.GetBytes(yml)) }
                };
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json"),
                    RequestUri = new Uri($"{faasConfiguration.Provider.Gateway}/configuration")
                };
                var httpResult = httpClient.SendAsync(request).Result;
                httpResult.EnsureSuccessStatusCode();
                Console.WriteLine("Configuration is applied");
            }
        }
    }
}
