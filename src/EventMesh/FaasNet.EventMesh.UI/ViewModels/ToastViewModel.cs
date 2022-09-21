namespace FaasNet.EventMesh.UI.ViewModels
{
	public class ToastViewModel
	{
		public string Title { get; set; }
		public string Message { get; set; }
		public DateTime ReceivedDateTime { get; set; }
		public ToastVerbosities Verbosity { get; set; }
		public DateTime ExpirationDateTime { get; set; }
		public bool IsExpired => DateTime.UtcNow > ExpirationDateTime;
    }

	public enum ToastVerbosities
	{
		INFO = 0,
		ERROR = 1
	}
}
