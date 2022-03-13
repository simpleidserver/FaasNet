using FaasNet.CLI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FaasNet.CLI.Commands
{
    public class FunctionCreateDockerFileCommand : IMenuItemCommand
    {
        public string Command => "-df";
        public string Description => "Create a docker file for the function";

        public void Execute(IEnumerable<string> args)
        {
            if (!args.Any())
            {
                Console.WriteLine("A directory must be specified");
                return;
            }

            var projectName = GetProjectName(args.First());
            if (string.IsNullOrWhiteSpace(projectName))
            {
                return;
            }

            var dockerFileContent = GetDockerFile(projectName);
            if (string.IsNullOrWhiteSpace(dockerFileContent))
            {
                return;
            }

            WriteDockerFile(args.First(), dockerFileContent);
        }

        protected static string GetProjectName(string directory)
        {
            if (!DockerHelper.CheckDockerExists())
            {
                Console.WriteLine("Docker is not installed");
                return null;
            }

            var projectName = ExtractProjectName(directory);
            if (string.IsNullOrWhiteSpace(projectName))
            {
                return null;
            }

            return projectName;
        }

        protected static string ExtractProjectName(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"The Directory '{directory}' doesn't exist");
                return null;
            }

            var files = Directory.GetFiles(directory, "*.csproj");
            if (!files.Any())
            {
                Console.WriteLine($"The Directory '{directory}' doesn't contain .csproj");
                return null;
            }

            if (files.Count() > 1)
            {
                Console.WriteLine($"The Directory '{directory}' contains more than one .csproj");
                return null;
            }

            return Path.GetFileName(files.First()).Replace(".csproj", string.Empty);
        }

        protected static string GetDockerFile(string projectName)
        {
            var dockerFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Files", "Dockerfile.txt");
            if(!File.Exists(dockerFilePath))
            {
                Console.WriteLine($"The Dockerfile '{dockerFilePath}' doesn't exist");
                return null;
            }

            var txt = File.ReadAllText(dockerFilePath);
            return txt.Replace("{{ProjectName}}", projectName);
        }

        protected static void WriteDockerFile(string directory, string content)
        {
            var dockerFileTargetPath = DockerHelper.GetDockefilePath(directory);
            if (File.Exists(dockerFileTargetPath))
            {
                File.Delete(dockerFileTargetPath);
            }

            File.WriteAllText(dockerFileTargetPath, content);
            Console.WriteLine($"The dockerfile has been written into '{dockerFileTargetPath}'");
        }
    }
}
