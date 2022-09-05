using FaasNet.Common.Helpers;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
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
        public async Task<BaseEventMeshPackage> Handle(AddTopicRequest addTopicRequest, CancellationToken cancellationToken)
        {
            var partition = await PartitionPeerStore.Get(addTopicRequest.Topic);
            if (partition != null) return PackageResponseBuilder.AddTopic(addTopicRequest.Seq, AddTopicStatus.EXISTING_TOPIC);
            return await Broadcast();

            async Task<BaseEventMeshPackage> Broadcast()
            {
                IEnumerable<ClusterPeer> filteredNodes = new List<ClusterPeer>();
                try
                {
                    var allNodes = (await ClusterStore.GetAllNodes("*", cancellationToken)).Where(c => c.Id != PeerOptions.Id);
                    var expectedNbActiveNodes = _eventMeshOptions.NbPartitionsTopic - 1;
                    if (allNodes.Count() < expectedNbActiveNodes) return PackageResponseBuilder.AddTopic(addTopicRequest.Seq, AddTopicStatus.NOT_ENOUGHT_ACTIVENODES);
                    filteredNodes = allNodes.OrderBy(o => Guid.NewGuid()).Take(expectedNbActiveNodes);
                    var addTopicResultLst = new ConcurrentBag<bool>();
                    await Parallel.ForEachAsync(filteredNodes, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = _eventMeshOptions.MaxNbThreads
                    }, async (n, t) =>
                    {
                        addTopicResultLst.Add(await AddTopic(addTopicRequest.Topic, n));
                    });
                    var nbSuccess = addTopicResultLst.Count(r => r);
                    if (nbSuccess < expectedNbActiveNodes)
                    {
                        await Rollback(filteredNodes);
                        return PackageResponseBuilder.AddTopic(addTopicRequest.Seq, AddTopicStatus.NOT_ENOUGHT_ACTIVENODES);
                    }
                    await PartitionCluster.TryAddAndStart(addTopicRequest.Topic, typeof(TopicMessageStateMachine));
                    return PackageResponseBuilder.AddTopic(addTopicRequest.Seq);
                }
                catch(Exception ex)
                {
                    await Rollback(filteredNodes);
                    Logger.LogError(ex.ToString());
                    return PackageResponseBuilder.AddTopic(addTopicRequest.Seq, AddTopicStatus.INTERNAL_ERROR);
                }
            }

            async Task<bool> AddTopic(string topic, ClusterPeer peer)
            {
                try
                {
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                    using (var eventMeshClient = _peerClientFactory.Build<PartitionClient>(edp))
                    {
                        var topicResult = await eventMeshClient.AddPartition(topic, typeof(TopicMessageStateMachine), _eventMeshOptions.RequestExpirationTimeMS, cancellationToken);
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
                    await RemoveTopic(addTopicRequest.Topic, n);
                });
                await PartitionCluster.TryRemove(addTopicRequest.Topic, TokenSource.Token);
            }

            async Task<bool> RemoveTopic(string topic, ClusterPeer peer)
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
