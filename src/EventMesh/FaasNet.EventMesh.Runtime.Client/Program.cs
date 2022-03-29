using FaasNet.EventMesh.Client;
using System;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Client
{
    class Program
    {
        private static SubscriptionResult _sub;
        private static UsersClient _subClient;

        static async Task<int> Main(string[] args)
        {
            await Subscribe();
            Console.WriteLine("Press enter to publish a message");
            Console.ReadLine();
            await Publish();
            Console.WriteLine("Press enter to quit the application");
            Console.ReadLine();
            _sub.Stop();
            _subClient.Disconnect();
            return 1;
        }

        private static async Task Subscribe()
        {
            _subClient = new UsersClient("subClientId", "password", port: 4000);
            _sub = await _subClient.SubscribeUserCreated((act) =>
            {
                Console.WriteLine($"FirstName received : {act.FirstName}");
            });
        }

        private static async Task Publish()
        {
            using (var usersClient = new UsersClient("pubClientId", "password", port: 4000))
            {
                await usersClient.Publish(new Models.UserCreatedEvent
                {
                    FirstName = "firstName",
                    LastName = "lastName"
                });
            }
        }
    }
}
