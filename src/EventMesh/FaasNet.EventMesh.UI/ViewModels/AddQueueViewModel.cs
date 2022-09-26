using System.ComponentModel.DataAnnotations;

namespace FaasNet.EventMesh.UI.ViewModels
{
	public class AddQueueViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Vpn { get; set; }
        [Required]
        public string TopicFilter { get; set; }

        public void Reset()
        {
            Name = string.Empty;
            Vpn = string.Empty;
            TopicFilter = string.Empty;
        }
	}
}
