using FaasNet.RaftConsensus.Client.Messages;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetPluginConfigurationResponse : Package
    {
        public GetPluginConfigurationResponse()
        {
            Records = new List<PluginConfigurationRecordResponse>();
        }

        public ICollection<PluginConfigurationRecordResponse> Records { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteInteger(Records.Count());
            foreach (var record in Records) record.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var result = new List<PluginConfigurationRecordResponse>();
            int length = context.NextInt();
            for (var i = 0; i < length; i++) result.Add(PluginConfigurationRecordResponse.Extract(context));
            Records = result;
        }
    }

    public class PluginConfigurationRecordResponse
    {
        public PluginConfigurationRecordResponse(string name, string description, string defaultValue, string configuredValue)
        {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
            ConfiguredValue = configuredValue;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string DefaultValue { get; private set; }
        public string ConfiguredValue { get; private set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Description);
            context.WriteString(DefaultValue);
            context.WriteString(ConfiguredValue);
        }

        public static PluginConfigurationRecordResponse Extract(ReadBufferContext context)
        {
            return new PluginConfigurationRecordResponse(context.NextString(), context.NextString(), context.NextString(), context.NextString());
        }
    }
}
