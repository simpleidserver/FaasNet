using System.Threading.Tasks;

namespace EventMesh.Runtime.Client
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // await Scenario1CreateSubSession.Launch();
            await Scenario2SubscribeToOneTopic.Launch();
            return 1;
        }
    }
}
