using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class UpdatePluginConfigurationRequest : Package
    {
        public string Name { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Name);
            context.WriteString(PropertyName);
            context.WriteString(PropertyValue);
        }

        public void Extract(ReadBufferContext context)
        {
            Name = context.NextString();
            PropertyName = context.NextString();
            PropertyValue = context.NextString();
        }
    }
}
