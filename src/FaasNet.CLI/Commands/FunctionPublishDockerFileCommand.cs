using FaasNet.CLI.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FaasNet.CLI.Commands
{
    public class FunctionPublishDockerFileCommand : IMenuItemCommand
    {
        public string Command => "-dp";
        public string Description => "Publish the docker image";

        public void Execute(IEnumerable<string> args)
        {
            if (!args.Any())
            {
                Console.WriteLine("A docker image must be specified");
                return;
            }
            
            if (!DockerHelper.CheckDockerExists())
            {
                Console.WriteLine("Docker is not installed");
                return;
            }

            var dockerImage = args.First();
            PublishDocker(dockerImage);
        }
        protected static void PublishDocker(string dockerImage)
        {
            using (var p = new Process())
            {
                p.StartInfo.FileName = "docker";
                p.StartInfo.Arguments = $"push {dockerImage}";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                while (!p.StandardOutput.EndOfStream)
                {
                    var line = p.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }

                p.WaitForExit();
            }
        }
    }
}
