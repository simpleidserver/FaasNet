using CloudNative.CloudEvents;
using EventMesh.Runtime.Models;
using System.Threading.Tasks;

namespace EventMesh.Runtime
{
    public interface IMessagePublisher
    {
        string BrokerName { get; }
        Task Publish(CloudEvent cloudEvent, string topicName, Client client);
    }
}
