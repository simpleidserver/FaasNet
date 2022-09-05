using FaasNet.Common.Helpers;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.Peer.Clusters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
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
            var partition = await _partitionPeerStore.Get(addTopicRequest.Topic);
            if (partition != null) return PackageResponseBuilder.AddTopic(addTopicRequest.Seq, AddTopicStatus.EXISTING_TOPIC);
            if (addTopicRequest.IsBroadcasted) return await Add(addTopicRequest.Topic);
            return await Broadcast();

            async Task<BaseEventMeshPackage> Add(string topic)
            {
                await PartitionCluster.TryAddAndStart(topic, typeof(TopicMessageStateMachine));
                return PackageResponseBuilder.AddTopic(addTopicRequest.Seq);
            }

            async Task<BaseEventMeshPackage> Broadcast()
            {
                var allNodes = (await ClusterStore.GetAllNodes("*", cancellationToken)).Where(c => c.Id != PeerOptions.Id);
                var expectedNbActiveNodes = _eventMeshOptions.NbPartitionsTopic - 1;
                if (allNodes.Count() < expectedNbActiveNodes) return PackageResponseBuilder.AddTopic(addTopicRequest.Seq, AddTopicStatus.NOT_ENOUGHT_ACTIVENODES);
                addTopicRequest.IsBroadcasted = true;
                var filteredNodes = allNodes.OrderBy(o => Guid.NewGuid()).Take(expectedNbActiveNodes);
                var addTopicResultLst = new ConcurrentBag<bool>();
                await Parallel.ForEachAsync(filteredNodes, new ParallelOptions
                {
                    MaxDegreeOfParallelism = _eventMeshOptions.MaxNbThreads
                }, async (n, t) =>
                {
                    addTopicResultLst.Add(await AddTopic(addTopicRequest.Topic, n));
                });
                var nbSuccess = addTopicResultLst.Count(r => r);
                if (nbSuccess < expectedNbActiveNodes) return PackageResponseBuilder.AddTopic(addTopicRequest.Seq, AddTopicStatus.NOT_ENOUGHT_ACTIVENODES);
                await PartitionCluster.TryAddAndStart(addTopicRequest.Topic, typeof(TopicMessageStateMachine));
                return PackageResponseBuilder.AddTopic(addTopicRequest.Seq);
            }

            async Task<bool> AddTopic(string topic, ClusterPeer peer)
            {
                try
                {
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                    using (var eventMeshClient = _peerClientFactory.Build<EventMeshClient>(edp))
                    {
                        var topicResult = await eventMeshClient.AddTopic(topic, true, _eventMeshOptions.RequestExpirationTimeMS, cancellationToken);
                        return topicResult.Status == AddTopicStatus.SUCCESS;
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
