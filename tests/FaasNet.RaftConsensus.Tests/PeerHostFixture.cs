﻿using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FaasNet.RaftConsensus.Tests
{
    public class PeerHostFixture
    {
        [Fact]
        public async Task When_AppendLog_Then_LogIsReplicated()
        {
            // ARRANGE
            var firstNode = BuildNodeHost(new ConcurrentBag<PeerInfo>
            {
                new PeerInfo { TermId = "termId", TermIndex = 0 }
            }, 4001, new ConcurrentBag<ClusterNode>
            {
                new ClusterNode
                {
                    Port = 4002,
                    Url = "localhost"
                }
            });
            var secondNode = BuildNodeHost(new ConcurrentBag<PeerInfo>
            {
                new PeerInfo { TermId = "termId", TermIndex = 0 }
            }, 4002, new ConcurrentBag<ClusterNode>
            {
                new ClusterNode
                {
                    Port = 4001,
                    Url = "localhost"
                }
            });
            await firstNode.Start(CancellationToken.None);
            await secondNode.Start(CancellationToken.None);
            var allNodes = new List<INodeHost> { firstNode, secondNode };

            // ACT
            WaitOnlyOneLeader(allNodes);
            var client = new ConsensusClient("localhost", 4001);
            client.AppendEntry("termId", "Key", "value", CancellationToken.None).Wait();
            WaitLogs(allNodes, p => p.Info.TermId == "termId", l => l.Key == "Key");

            // ASSERT
            var firstPeerLogs = firstNode.Peers.First().LogStore.GetAll(CancellationToken.None).Result;
            var secondPeerLogs = secondNode.Peers.First().LogStore.GetAll(CancellationToken.None).Result;
            Assert.Single(firstPeerLogs);
            Assert.Single(secondPeerLogs);
            Assert.Equal("Key", firstPeerLogs.First().Key);
            Assert.Equal("Key", secondPeerLogs.First().Key);
            await firstNode.Stop();
            await secondNode.Stop();
        }

        private static INodeHost BuildNodeHost(ConcurrentBag<PeerInfo> peers, int port, ConcurrentBag<ClusterNode> clusterNodes)
        {
            var serviceProvider = new ServiceCollection()
                .AddConsensusPeer(o => o.Port = port)
                .SetPeers(peers)
                .SetClusterNodes(clusterNodes)
                .Services
                .BuildServiceProvider();
            return serviceProvider.GetService<INodeHost>();
        }

        private static void WaitOnlyOneLeader(List<INodeHost> nodes)
        {
            while (true)
            {
                var peers = nodes.SelectMany(n => n.Peers);
                var leaderNodes = peers.Where(p => p.State == PeerStates.LEADER);
                var followerNodes = peers.Where(p => p.State == PeerStates.FOLLOWER && p.ActiveNode != null);
                if (leaderNodes.Count() == 1 && followerNodes.Count() == peers.Count() - 1)
                {
                    return;
                }

                Thread.Sleep(200);
            }
        }

        private static void WaitLogs(List<INodeHost> nodes, Func<IPeerHost, bool> callbackPeers, Func<LogRecord, bool> callbackLogRecords)
        {
            var isLogPropagated = false;
            while(!isLogPropagated)
            {
                isLogPropagated = true;
                foreach(var node in nodes)
                {
                    var filteredPeers = node.Peers.Where(callbackPeers);
                    foreach(var filteredPeer in filteredPeers)
                    {
                        var filteredLogs = filteredPeer.LogStore.GetAll(CancellationToken.None).Result.Where(callbackLogRecords);
                        if (!filteredLogs.Any()) isLogPropagated = false;
                    }
                }

                Thread.Sleep(200);
            }
        }
    }
}
