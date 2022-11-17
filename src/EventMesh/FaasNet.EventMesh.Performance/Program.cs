using FaasNet.EventMesh.Performance;
using FaasNet.EventMesh.Performance.Scenarios;

var benchmark = new EventMeshBenchmarkLauncher();
await benchmark.Launch(new IBenchmarkScenario[]
{
    new PublishAndReceiveMessageScenario(600),
    // new ReceiveMessageInNewNodeScenario()
});
Console.ReadLine();