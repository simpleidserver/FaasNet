using FaasNet.EventMesh.Common;

namespace FaasNet.EventMesh.SecondConsole
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Second console";
            ConsoleHelper.Start(6000, 5673, 2804).Wait();
        }
    }
}
