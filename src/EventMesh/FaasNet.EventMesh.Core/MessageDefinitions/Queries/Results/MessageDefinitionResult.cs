using FaasNet.EventMesh.Runtime.Models;
using System;

namespace FaasNet.EventMesh.Core.MessageDefinitions.Queries.Results
{
    public class MessageDefinitionResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string JsonSchema { get; set; }

        public static MessageDefinitionResult Build(MessageDefinition message)
        {
            return new MessageDefinitionResult
            {
                CreateDateTime = message.CreateDateTime,
                Description = message.Description,
                Id = message.Id,
                JsonSchema = message.JsonSchema,
                Name = message.Name,
                UpdateDateTime = message.UpdateDateTime,
                Version = message.Version
            };
        }
    }
}
