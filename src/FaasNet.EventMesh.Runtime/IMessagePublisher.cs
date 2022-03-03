using CloudNative.CloudEvents;
using FaasNet.EventMesh.Runtime.Models;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime
{
    public interface IMessagePublisher
    {
        string BrokerName { get; }
        Task Publish(CloudEvent cloudEvent, string topicName, Client client);
    }
}
