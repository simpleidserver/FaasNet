using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.App
{
    [FeatureState]
	public class AppState
	{
        public AppState()
        {

        }

		public EventMeshNode SelectedNode { get; set; }
        public IEnumerable<EventMeshNode> Nodes { get; set; } = new List<EventMeshNode>();
        public DateTime? LastRefreshTime { get; set; }
        public bool IsLoading { get; set; }
        public bool IsActive { get; set; }
    }

	public class EventMeshNode
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
        public string DisplayName
        {
            get
            {
                return $"{Url}:{Port}";
            }
        }
    }
}
