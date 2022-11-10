using FaasNet.EventMesh.Performance;

var benchmark = new EventMeshBenchmark();
await benchmark.Launch(300);
Console.ReadLine();