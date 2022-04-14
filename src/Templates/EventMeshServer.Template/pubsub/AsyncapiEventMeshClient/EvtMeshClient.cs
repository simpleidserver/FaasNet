using AsyncapiEventMeshClient.Models;
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
	
		
			public Task Publish(AnonymousSchema_1 parameter, CancellationToken cancellationToken = default(CancellationToken))
			{
				const string topicName = "e59f4ca1-73a3-4c93-b54a-9c04778bea72/User";
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
			
	}
}