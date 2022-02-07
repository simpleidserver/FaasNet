using CloudNative.CloudEvents;
using System.Threading.Tasks;

namespace EventMesh.Runtime
{
    public interface IMessagePublisher
    {
        Task Publish(CloudEvent cloudEvent, string topicName);
    }
}
