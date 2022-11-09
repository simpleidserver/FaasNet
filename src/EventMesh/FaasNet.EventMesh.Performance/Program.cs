using BenchmarkDotNet.Running;
using FaasNet.EventMesh.Performance;

var summary = BenchmarkRunner.Run<EventMeshBenchmark>();
var jobs = summary.BenchmarksCases.Select(b => b.Job);