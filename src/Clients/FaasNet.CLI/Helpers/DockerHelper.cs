using System.Diagnostics;
using System.IO;

namespace FaasNet.CLI.Helpers
{
    public class DockerHelper
    {
        public static bool CheckDockerExists()
        {
            try
            {
                var p = Process.Start("docker");
                p.Kill();
                p.WaitForExit();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetDockefilePath(string directory)
        {
            return Path.Combine(directory, "Dockerfile");
        }
    }
}
