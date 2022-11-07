using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using Saunter.AsyncApiSchema.v2;
using Saunter.Serialization;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetAsyncApiRequest getAsyncApiRequest, CancellationToken cancellationToken)
        {
            var clientResult = await Query<GetClientQueryResult>(PartitionNames.CLIENT_PARTITION_KEY, new GetClientQuery { Id = getAsyncApiRequest.ClientId, Vpn = getAsyncApiRequest.Vpn }, cancellationToken);
            if (!clientResult.Success) return PackageResponseBuilder.GetAsyncApi(getAsyncApiRequest.Seq, GetAsyncApiResultStatus.NOT_FOUND);
            var sourceEvts = await GetEvts(clientResult.Client.Sources.Select(s => s.EventId), getAsyncApiRequest.Vpn, cancellationToken);
            var targetEvts = await GetEvts(clientResult.Client.Targets.Select(s => s.EventId), getAsyncApiRequest.Vpn, cancellationToken);
            var doc = BuildDocumentation(clientResult.Client, sourceEvts, targetEvts);
            var serializer = new NewtonsoftAsyncApiDocumentSerializer();
            return PackageResponseBuilder.GetAsyncApi(getAsyncApiRequest.Seq, serializer.Serialize(doc));
        }

        async Task<IEnumerable<GetEventDefinitionQueryResult>> GetEvts(IEnumerable<string> ids, string vpn, CancellationToken cancellationToken)
        {
            var result = new ConcurrentBag<GetEventDefinitionQueryResult>();
            await Parallel.ForEachAsync(ids, new ParallelOptions
            {
                MaxDegreeOfParallelism = _options.MaxConcurrentThreads
            }, async (cmd, t) =>
            {
                var r = await Query<GetEventDefinitionQueryResult>(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new GetEventDefinitionQuery { Id = cmd, Vpn = vpn }, cancellationToken);
                result.Add(r);
            });
            return result;
        }

        AsyncApiDocument BuildDocumentation(ClientQueryResult client, IEnumerable<GetEventDefinitionQueryResult> sourceEvts, IEnumerable<GetEventDefinitionQueryResult> targetEvts)
        {
            var sourceMessages = TransformMessages(sourceEvts);
            var sourceChannels = TransformChannels(sourceEvts);
            var targetMessages = TransformMessages(targetEvts);
            var targetChannels = TransformChannels(targetEvts, false);
            var messages = sourceMessages.Union(targetMessages).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var channels = sourceChannels.Union(targetChannels).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return new AsyncApiDocument
            {
                Info = new Info(client.Id, "1.0.0"),
                Channels = channels,
                Components = new Components
                {
                    Messages = messages
                }
            };

            Dictionary<string, Message> TransformMessages(IEnumerable<GetEventDefinitionQueryResult> evts)
            {
                return evts.Select(e => new KeyValuePair<string, Message>(e.EventDef.Id, new Message
                {
                    Payload = NJsonSchema.JsonSchema.FromJsonAsync(e.EventDef.JsonSchema).Result
                }
                )).DistinctBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            Dictionary<string, ChannelItem> TransformChannels(IEnumerable<GetEventDefinitionQueryResult> evts, bool isSub = true)
            {
                return evts.Select(e =>
                {
                    var op = new Operation { Message = new MessageReference(e.EventDef.Id) };
                    ChannelItem channelItem;
                    if (isSub) channelItem = new ChannelItem
                    {
                        Subscribe = op
                    };
                    else channelItem = new ChannelItem
                    {
                        Publish = op
                    };
                    
                    return new KeyValuePair<string, ChannelItem>(e.EventDef.Topic, channelItem);
                }).DistinctBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }
    }
}
