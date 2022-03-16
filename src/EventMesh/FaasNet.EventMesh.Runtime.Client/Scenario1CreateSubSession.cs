using FaasNet.EventMesh.Runtime.Messages;
using System;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Client
{
    public static class Scenario1CreateSubSession
    {
        private static RuntimeClient _runtimeClient;

        public static async Task Launch(string hostName = "localhost", int port = 4000, string clientId = "clientId")
        {
            _runtimeClient = new RuntimeClient(hostName, port);
            var sessionId = await CreateSession(clientId);
            Console.WriteLine($"session is created with id '{sessionId}'");
            Console.WriteLine("Please press enter to quit the application ...");
            Console.ReadLine();
            await _runtimeClient.Disconnect(clientId, sessionId);
        }

        private static async Task<string> CreateSession(string clientId)
        {
            var helloResponse = await _runtimeClient.Hello(new UserAgent
            {
                ClientId = clientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                BufferCloudEvents = 2,
                Version = "0",
                Purpose = UserAgentPurpose.SUB,
                Vpn = "default"
            });
            return helloResponse.SessionId;
        }
    }
}
