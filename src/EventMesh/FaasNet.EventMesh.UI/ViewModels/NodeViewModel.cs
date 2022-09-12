namespace FaasNet.EventMesh.UI.ViewModels
{
	public class NodeViewModel
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
