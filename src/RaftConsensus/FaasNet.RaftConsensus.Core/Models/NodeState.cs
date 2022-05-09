namespace FaasNet.RaftConsensus.Core.Models
{
    public class NodeState
    {
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public int EntityVersion { get; set; }
        public string Value { get; set; }

        public void Update(string value)
        {
            Value = value;
            EntityVersion++;
        }

        public static NodeState Create(string entityType, string entityId, string value, int version = 0)
        {
            return new NodeState
            {
                EntityId = entityId,
                EntityType = entityType,
                EntityVersion = version,
                Value = value
            };
        }
    }
}
