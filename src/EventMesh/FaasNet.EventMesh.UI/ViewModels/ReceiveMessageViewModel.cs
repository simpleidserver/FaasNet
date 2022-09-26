using System.ComponentModel.DataAnnotations;

namespace FaasNet.EventMesh.UI.ViewModels
{
    public class ReceiveMessageViewModel
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        [Required]
        public string Vpn { get; set; }
        [Required]
        public string QueueName { get; set; }
    }
}
