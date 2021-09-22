using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace FaasNet.CLI.Commands
{
    public class ApplyHttpCommand : IMenuItemCommand
    {
        public string Command => "-u";

        public string Description => "Download and apply the file configuration";

        public void Execute(IEnumerable<string> args)
        {
            var path = GetFileContent(args);
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            ApplyCommand.Apply(path);
            File.Delete(path);
        }

        protected string GetFileContent(IEnumerable<string> args)
        {
            if (!args.Any())
            {
                Console.Write("A URL must be specified");
                return null;
            }

            var url = args.First();
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                Console.WriteLine($"The parameter '{url}' is not a valid URL");
                return null;
            }

            string content;
            using (var httpClient = new HttpClient())
            {
                var httpResponse = httpClient.GetAsync(url).Result;
                content = httpResponse.Content.ReadAsStringAsync().Result;
            }

            var tmp = Path.GetTempFileName();
            File.WriteAllText(tmp, content);
            return tmp;
        }
    }
}
