using System;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Client
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            await Scenario1SubscribeBridge.Launch();
            Console.WriteLine("Please press enter to quit the application ...");
            Console.ReadLine();
            return 1;
        }
    }
}
