using System;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class MessageDefinition
    {
        public string Id { get; set; }
        public string ApplicationDomainId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string JsonSchema { get; set; }

        public MessageDefinition Publish()
        {
            var result = new MessageDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = Name,
                Description = Description,
                Version = Version + 1,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                JsonSchema = JsonSchema
            };
            return result;
        }

        public void Update(string description, string jsonSchema)
        {
            Description = description;
            JsonSchema = jsonSchema;
            UpdateDateTime = DateTime.UtcNow;
        }

        public static MessageDefinition Create(string applicationDomainId, string name, string description, string jsonSchema)
        {
            var result = new MessageDefinition
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationDomainId = applicationDomainId,
                Name = name,
                Description = description,
                JsonSchema = jsonSchema,
                Version = 0,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            return result;
        }
    }
}
