using System.ComponentModel.DataAnnotations;

namespace FaasNet.EventMesh.UI.ViewModels
{
	public class AddVpnViewModel
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Description { get; set; }

		public void Reset()
		{
			Name = String.Empty;
			Description = String.Empty;
		}
	}
}
