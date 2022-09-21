using FaasNet.EventMesh.UI.ViewModels;

namespace FaasNet.EventMesh.UI.Data
{
    public class ToastService
    {
		private static object _lock = new object();
        private System.Timers.Timer _timer = new System.Timers.Timer();

        public ToastService()
		{
			_timer.Elapsed += (o, e) => CheckLifetime();
			_timer.AutoReset = false;
			_timer.Interval = 1000;
			_timer.Start();
        }

        public List<ToastViewModel> Toasts { get; private set; } = new List<ToastViewModel>();
        public event EventHandler<EventArgs> ToastsChanged;

        public void AddInfo(string title, string message)
		{
			Toasts.Add(new ToastViewModel
			{
				Title = title,
				Message	 = message,
				ReceivedDateTime = DateTime.UtcNow,
				Verbosity = ToastVerbosities.INFO,
				ExpirationDateTime = DateTime.UtcNow.AddSeconds(60)
			});
			if (ToastsChanged != null) ToastsChanged(ToastsChanged, EventArgs.Empty);
        }

        public void AddError(string title, string message)
        {
            Toasts.Add(new ToastViewModel
            {
                Title = title,
                Message = message,
                ReceivedDateTime = DateTime.UtcNow,
                Verbosity = ToastVerbosities.ERROR,
                ExpirationDateTime = DateTime.UtcNow.AddSeconds(60)
            });
            if (ToastsChanged != null) ToastsChanged(ToastsChanged, EventArgs.Empty);
        }

        public void Close(ToastViewModel toast)
		{
			lock(_lock)
				Toasts.Remove(toast);
            if (ToastsChanged != null) ToastsChanged(ToastsChanged, EventArgs.Empty);
        }

        private void CheckLifetime()
		{
			lock(_lock)
				Toasts = Toasts.Where(t => !t.IsExpired).ToList();
            if (ToastsChanged != null) ToastsChanged(ToastsChanged, EventArgs.Empty);
            _timer.Start();
		}
    }
}
