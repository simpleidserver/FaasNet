import _ from 'lodash';
import { File } from '@asyncapi/generator-react-sdk';
import { Channel } from '@asyncapi/parser';
import { FormatHelpers } from '@asyncapi/modelina';

function getChannelWrappers(channels) {
	const channelWrappers = [];
	for (const [channelName, channel] of channels) {
		const channelParameterEntries = Object.entries(channel.parameters());
		if (channel.hasSubscribe()) {
			const message = channel.subscribe().message(0);
			const messageType = FormatHelpers.toPascalCase(message.payload().uid());
			let msg = _.camelCase(channelName);
			msg = msg.charAt(0).toUpperCase() + msg.slice(1);
			const record = `
			public Task<SubscriptionResult> Subscribe${msg}(Action<${messageType}> callback, CancellationToken cancellationToken = default(CancellationToken))
			{
				return SubscribeMessages("${channelName}", callback, cancellationToken);
			}
			`;
			channelWrappers.push(record);
		}
		
		if (channel.hasPublish()) {
			const message = channel.publish().message(0);
			const messageType = FormatHelpers.toPascalCase(message.payload().uid());
			const record = `
			public Task Publish(${messageType} parameter, CancellationToken cancellationToken = default(CancellationToken))
			{
				const string topicName = "${channelName}";				
				var cloudEvt = new CloudEvent
				{
					Id = Guid.NewGuid().ToString(),
					Subject = topicName,
					Source = new Uri(parameter.Source),
					Type = parameter.Type,
					DataContentType = "application/json",
					Data = JsonSerializer.Serialize(parameter),
					Time = DateTimeOffset.UtcNow
				};
				return Publish(topicName, cloudEvt, cancellationToken);
			}
			`;
			channelWrappers.push(record);
		}
	}
  
	return channelWrappers;
}

export default function evtMeshClient({ asyncapi }) {
  const channelIterator = Object.entries(asyncapi.channels());
  const channels = getChannelWrappers(channelIterator);
  return <File name={'EvtMeshClient.cs'}>
    {
      `using AsyncapiEventMeshClient.Models;
using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncapiEventMeshClient
{
	public class EvtMeshClient : EventMeshClient
	{
		public EvtMeshClient(string clientId, string password, string vpn = Constants.DefaultVpn, string url = Constants.DefaultUrl, int port = Constants.DefaultPort, int bufferCloudEvents = 1) : base(clientId, password, vpn, url, port, bufferCloudEvents) { }
	
		${channels.join('\n')}
	}
}`
    }
  </File>;
}