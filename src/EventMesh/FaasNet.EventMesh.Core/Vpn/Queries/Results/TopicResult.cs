using FaasNet.EventMesh.Runtime.Models;

namespace FaasNet.EventMesh.Core.Vpn.Queries.Results
{
    public class TopicResult
    {
        public string BrokerName { get; set; }
        public string Name { get; set; }

        public static TopicResult Build(Topic topic)
        {
            return new TopicResult
            {
                BrokerName = topic.BrokerName,
                Name = topic.Name
            };
        }
    }
}
