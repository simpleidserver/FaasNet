namespace FaasNet.RaftConsensus.Service
{
    internal partial class Program
    {
        public static int Main(string[] args)
        {
            // LaunchNodes();
            LaunchPeers();
            Console.WriteLine("Press any key to quit the application");
            Console.ReadLine();
            return 1;
        }
    }
}
