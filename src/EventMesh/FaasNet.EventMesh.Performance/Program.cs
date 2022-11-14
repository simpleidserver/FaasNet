using FaasNet.EventMesh.Performance;
using System;
using System.Text;

var benchmark = new EventMeshBenchmark();
await benchmark.Launch(300);
Console.ReadLine();