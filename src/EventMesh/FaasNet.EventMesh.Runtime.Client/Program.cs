﻿using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Client
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // await Scenario1CreateSubSession.Launch();
            await Scenario2SubscribeToOneTopic.Launch(port: 30005);
            // await Scenario3SubscribeToOneTopicAndPublishMessage.Launch();
            return 1;
        }
    }
}