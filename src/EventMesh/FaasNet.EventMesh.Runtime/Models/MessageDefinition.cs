using System;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class MessageDefinition
    {
        public string Id { get; set; }
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
                Name = Name,
                Description = Description,
                Version = Version + 1,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                JsonSchema = JsonSchema
            };
            result.Id = BuildId(result);
            return result;
        }

        public static MessageDefinition Create(string name, string description, string jsonSchema)
        {
            var result = new MessageDefinition
            {
                Name = name,
                Description = description,
                JsonSchema = jsonSchema,
                Version = 0,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            result.Id = BuildId(result);
            return result;
        }

        public static string BuildId(MessageDefinition message)
        {
            return $"{message.Name}{message.Version}";
        }
    }
}
