using System.ComponentModel.DataAnnotations;

namespace FaasNet.EventMesh.UI.ViewModels
{
    public class PublishMessageViewModel
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        [Required]
        public string Vpn { get; set; }
        [Required]
        public string Topic { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
