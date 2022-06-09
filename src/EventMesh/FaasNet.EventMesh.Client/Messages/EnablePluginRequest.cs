using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class EnablePluginRequest: Package
    {
        public string PluginName { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(PluginName);
        }

        public void Extract(ReadBufferContext context)
        {
            PluginName = context.NextString();
        }
    }
}
