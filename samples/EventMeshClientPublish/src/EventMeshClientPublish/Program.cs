using CloudNative.CloudEvents;
using FaasNet.EventMesh.Runtime;
using FaasNet.EventMesh.Runtime.Messages;
using System;
using System.Threading.Tasks;

namespace EventMeshClientPublish
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var runtimeClient = new RuntimeClient("localhost", 4000);
            var sessionId = CreateSession(runtimeClient).Result;
            PublishMessage(runtimeClient, sessionId).Wait();
            Console.WriteLine("Please press enter to quit the application ...");
            Console.ReadLine();
            runtimeClient.Disconnect("pubClientId", sessionId).Wait();
        }

        private static async Task<string> CreateSession(RuntimeClient runtimeClient)
        {
            var helloResponse = await runtimeClient.Hello(new UserAgent
            {
                ClientId = "pubClientId",
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                BufferCloudEvents = 1,
                Version = "0",
                Purpose = UserAgentPurpose.PUB
            });
            return helloResponse.SessionId;
        }
        private static Task<Package> PublishMessage(RuntimeClient runtimeClient, string sessionId)
        {
            var cloudEvent = new CloudEvent
            {
                Type = "com.github.pull.create",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = "person is created",
                ["comexampleextension1"] = "value"
            };
            return runtimeClient.PublishMessage("pubClientId", sessionId, "Person.Created", cloudEvent);
        }
    }
}
