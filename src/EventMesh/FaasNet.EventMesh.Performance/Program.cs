using BenchmarkDotNet.Running;
using FaasNet.EventMesh.Performance;
using FaasNet.Peer.Clusters;
using System.Collections.Concurrent;

var clusterNodes = new ConcurrentBag<ClusterPeer>();
var firstNode = await NodeHelper.BuildAndStartNode(5000, clusterNodes, 30000);
var secondNode = await NodeHelper.BuildAndStartNode(5001, clusterNodes, 40000);
var summary = BenchmarkRunner.Run<EventMeshBenchmark>();
// await firstNode.Stop();
// await secondNode.Start();