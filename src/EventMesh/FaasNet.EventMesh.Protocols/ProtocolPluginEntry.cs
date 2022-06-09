using FaasNet.EventMesh.Plugin;

namespace FaasNet.EventMesh.Protocols
{
    public class ProtocolPluginEntry : PluginEntry
    {
        public string DllName { get; set; }
        public object Configuration { get; set; }
    }
}
