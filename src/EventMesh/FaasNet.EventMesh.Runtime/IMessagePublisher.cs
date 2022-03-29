using CloudNative.CloudEvents;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime
{
    public interface IMessagePublisher
    {
        string BrokerName { get; }
        Task Publish(CloudEvent cloudEvent, string topicName, Models.Client client);
    }
}
