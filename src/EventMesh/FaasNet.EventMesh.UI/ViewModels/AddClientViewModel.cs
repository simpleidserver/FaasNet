using System.ComponentModel.DataAnnotations;

namespace FaasNet.EventMesh.UI.ViewModels
{
    public class AddClientViewModel
	{
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string Vpn { get; set; }
        [Required]
        public int[] PurposeTypes { get; set; } = new int[] { };

        public void Reset()
        {
            ClientId = String.Empty;
            Vpn = String.Empty;
            PurposeTypes = new int[] { };
        }
    }
}
