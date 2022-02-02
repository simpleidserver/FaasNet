using System;

namespace EventMesh.Runtime.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            LaunchRuntime();
            Console.WriteLine("Please press Enter to quit the application...");
            Console.ReadLine();
        }

        private static void LaunchRuntime()
        {
            Console.WriteLine("Launch EventMesh runtime...");
            
            Console.WriteLine("EventMesh runtime is launched !");
        }
    }
}
