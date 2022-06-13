using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetPluginConfigurationRequest : Package
    {
        public string Name { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Name);
        }

        public void Extract(ReadBufferContext context)
        {
            Name = context.NextString();
        }
    }
}
