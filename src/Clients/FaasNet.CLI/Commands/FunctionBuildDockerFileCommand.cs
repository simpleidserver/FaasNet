using FaasNet.CLI.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FaasNet.CLI.Commands
{
    public class FunctionBuildDockerFileCommand : IMenuItemCommand
    {
        public string Command => "-db";
        public string Description => "Build docker file";

        public void Execute(IEnumerable<string> args)
        {
            if (!args.Any())
            {
                Console.WriteLine("A directory must be specified");
                return;
            }

            if (!DockerHelper.CheckDockerExists())
            {
                Console.WriteLine("Docker is not installed");
                return;
            }

            var directory = args.First();
            var dockerFileTargetPath = GetDockerFilePath(directory);
            if (string.IsNullOrWhiteSpace(dockerFileTargetPath))
            {
                return;
            }

            BuildDocker(dockerFileTargetPath, directory, args);
        }

        protected static string GetDockerFilePath(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"The Directory '{directory}' doesn't exist");
                return null;
            }

            var dockerFileTargetPath = DockerHelper.GetDockefilePath(directory);
            if (!File.Exists(dockerFileTargetPath))
            {
                Console.WriteLine($"There is no Dockerfile in the directory '{dockerFileTargetPath}'");
                return null;
            }

            return dockerFileTargetPath;
        }

        protected static void BuildDocker(string dockerFileTargetPath, string directory, IEnumerable<string> args)
        {
            args = args.Skip(1);
            string addArgs = args.Any() ? string.Join(" ", args) : string.Empty;
            using (var p = new Process())
            {
                p.StartInfo.FileName = "docker";
                p.StartInfo.Arguments = $"build -f {dockerFileTargetPath} {addArgs} {directory}";
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
