using FaasNet.Common.Helpers;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using FaasNet.EventMesh.StateMachines.QueueMessage;
using FaasNet.Partition;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Messages;
using FaasNet.Peer.Clusters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddQueueRequest addQueueRequest, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = addQueueRequest.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.AddQueue(addQueueRequest.Seq, AddQueueStatus.UNKNOWN_VPN);
            var queue = await Query<GetQueueQueryResult>(PartitionNames.QUEUE_PARTITION_KEY, new GetQueueQuery 
            { 
                QueueName = addQueueRequest.QueueName,
                Vpn = addQueueRequest.Vpn
            }, cancellationToken);
            if (queue.Success) return PackageResponseBuilder.AddQueue(addQueueRequest.Seq, AddQueueStatus.EXISTING_QUEUE);
            var broadcastResult = await BroadcastPartitions();
            if (!broadcastResult.Item1)
            {
                await Rollback(broadcastResult.Item3);
                return broadcastResult.Item2;
            }

            await AddQueue();
            return  PackageResponseBuilder.AddQueue(addQueueRequest.Seq);

            async Task<(bool, BaseEventMeshPackage, IEnumerable<ClusterPeer>)> BroadcastPartitions()
            {
                IEnumerable<ClusterPeer> filteredNodes = new List<ClusterPeer>();
                try
                {
                    var allNodes = (await ClusterStore.GetAllNodes(PartitionedNodeHost.PARTITION_KEY, cancellationToken)).Where(c => c.Id != PeerOptions.Id);
                    var expectedNbActiveNodes = _eventMeshOptions.NbPartitionsTopic - 1;
                    if (allNodes.Count() < expectedNbActiveNodes) return (false, PackageResponseBuilder.AddQueue(addQueueRequest.Seq, AddQueueStatus.NOT_ENOUGHT_ACTIVENODES), filteredNodes);
                    filteredNodes = allNodes.OrderBy(o => Guid.NewGuid()).Take(expectedNbActiveNodes);
                    var addTopicResultLst = new ConcurrentBag<bool>();
                    await Parallel.ForEachAsync(filteredNodes, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = _eventMeshOptions.MaxNbThreads
                    }, async (n, t) =>
                    {
                        addTopicResultLst.Add(await AddPartition($"{addQueueRequest.Vpn}_{addQueueRequest.QueueName}", n));
                    });
                    var nbSuccess = addTopicResultLst.Count(r => r);
                    if (nbSuccess < expectedNbActiveNodes) return (false, PackageResponseBuilder.AddQueue(addQueueRequest.Seq, AddQueueStatus.NOT_ENOUGHT_ACTIVENODES), filteredNodes);
                    await PartitionCluster.TryAddAndStart($"{addQueueRequest.Vpn}_{addQueueRequest.QueueName}", typeof(QueueMessageStateMachine));
                    return (true, PackageResponseBuilder.AddQueue(addQueueRequest.Seq), filteredNodes);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    return (false, PackageResponseBuilder.AddQueue(addQueueRequest.Seq, AddQueueStatus.INTERNAL_ERROR), filteredNodes);
                }
            }

            async Task AddQueue()
            {
                var addQueueCommand = new AddQueueCommand { QueueName = addQueueRequest.QueueName, Vpn = addQueueRequest.Vpn };
                await Send(PartitionNames.QUEUE_PARTITION_KEY, addQueueCommand, cancellationToken);
            }

            async Task<bool> AddPartition(string topic, ClusterPeer peer)
            {
                try
                {
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                    using (var eventMeshClient = _peerClientFactory.Build<PartitionClient>(edp))
                    {
                        var topicResult = await eventMeshClient.AddPartition(topic, typeof(QueueMessageStateMachine), _eventMeshOptions.RequestExpirationTimeMS, cancellationToken);
                        return topicResult.Status == AddDirectPartitionStatus.SUCCESS;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    return false;
                }
            }

            async Task Rollback(IEnumerable<ClusterPeer> peers)
            {
                await Parallel.ForEachAsync(peers, new ParallelOptions
                {
                    MaxDegreeOfParallelism = _eventMeshOptions.MaxNbThreads
                }, async (n, t) =>
                {
                    await RemovePartition($"{addQueueRequest.Vpn}_{addQueueRequest.QueueName}", n);
                });
                await PartitionCluster.TryRemove($"{addQueueRequest.Vpn}_{addQueueRequest.QueueName}", TokenSource.Token);
            }

            async Task<bool> RemovePartition(string topic, ClusterPeer peer)
            {
                try
                {
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                    using (var eventMeshClient = _peerClientFactory.Build<PartitionClient>(edp))
                    {
                        var topicResult = await eventMeshClient.RemovePartition(topic, _eventMeshOptions.RequestExpirationTimeMS, cancellationToken);
                        return topicResult.Status == RemoveDirectPartitionStatus.SUCCESS;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    return false;
                }
            }
        }
    }
}
