using FaasNet.RaftConsensus.Client;

while(true)
{

    Console.WriteLine("Press enter to send a message");
    Console.ReadLine();

    var client = new ConsensusClient("localhost", 4001);
    client.AppendEntry("log-2022-01", "Value2", CancellationToken.None).Wait();
}

Console.WriteLine("Press Enter to quit the application");
Console.ReadLine();