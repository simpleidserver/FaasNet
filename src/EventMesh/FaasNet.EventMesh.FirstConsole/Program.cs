using FaasNet.EventMesh.Common;

namespace FaasNet.EventMesh.FirstConsole
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ConsoleHelper.Start(4000, 5672).Wait();
        }
    }
}
