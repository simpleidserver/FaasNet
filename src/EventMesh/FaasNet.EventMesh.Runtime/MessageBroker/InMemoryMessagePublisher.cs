using CloudNative.CloudEvents;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public class InMemoryMessagePublisher : IMessagePublisher
    {
        private readonly IEventMeshCloudEventRepository _eventMeshCloudEventRepository;

        public InMemoryMessagePublisher(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            _eventMeshCloudEventRepository = scope.ServiceProvider.GetRequiredService<IEventMeshCloudEventRepository>();
        }

        public string BrokerName => Constants.InMemoryBrokername;

        public async Task Publish(CloudEvent cloudEvent, string topicName, Models.Client client)
        {
            _eventMeshCloudEventRepository.Add(new EventMeshCloudEvent { CloudEvt = cloudEvent, Topic = topicName, CreateDateTime = DateTime.UtcNow });
            await _eventMeshCloudEventRepository.SaveChanges();
        }
    }
}
