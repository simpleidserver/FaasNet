using FaasNet.RaftConsensus.Client.Messages;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllPluginsResponse : Package
    {
        public GetAllPluginsResponse()
        {
            Plugins = new List<PluginResponse>();
        }

        public ICollection<PluginResponse> Plugins { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteInteger(Plugins.Count());
            foreach(var plugin in Plugins) plugin.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var plugins = new List<PluginResponse>();
            var nb = context.NextInt();
            for (int i = 0; i < nb; i++) plugins.Add(PluginResponse.Extract(context));
            Plugins = plugins;
        }
    }

    public class PluginResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Description);
            context.WriteBoolean(IsActive);
        }

        public static PluginResponse Extract(ReadBufferContext context)
        {
            return new PluginResponse
            {
                Name = context.NextString(),
                Description = context.NextString(),
                IsActive = context.NextBoolean()
            };
        }
    }
}
