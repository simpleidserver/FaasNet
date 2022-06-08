namespace FaasNet.EventMesh.Sink
{
    public class SinkPluginEntry
    {
        public string DllName { get; set; }
        public object SinkOptions { get; set; }
        public object PluginOptions { get; set; }
    }
}
