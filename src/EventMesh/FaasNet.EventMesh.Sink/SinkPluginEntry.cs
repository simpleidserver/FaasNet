using FaasNet.EventMesh.Plugin;

namespace FaasNet.EventMesh.Sink
{
    public class SinkPluginEntry : PluginEntry
    {
        public string DllName { get; set; }
        public object SinkOptions { get; set; }
        public object PluginOptions { get; set; }
    }
}
