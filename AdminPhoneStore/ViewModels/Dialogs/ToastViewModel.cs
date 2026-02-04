using AdminPhoneStore.Services.UI;
using AdminPhoneStore.ViewModels.Base;
using System.Windows.Media;
using System.Windows.Threading;

namespace AdminPhoneStore.ViewModels.Dialogs
{
    public class ToastViewModel : BaseViewModel
    {
        private string _message = string.Empty;
        private ToastType _toastType = ToastType.Info;
        private DispatcherTimer? _timer;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public ToastType ToastType
        {
            get => _toastType;
            set
            {
                _toastType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Icon));
                OnPropertyChanged(nameof(IconColor));
            }
        }

        public string Icon
        {
            get
            {
                return _toastType switch
                {
                    ToastType.Success => "✓",
                    ToastType.Error => "✕",
                    ToastType.Warning => "⚠",
                    ToastType.Info => "ℹ",
                    _ => "ℹ"
                };
            }
        }

        public Brush IconColor
        {
            get
            {
                return _toastType switch
                {
                    ToastType.Success => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")),
                    ToastType.Error => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336")),
                    ToastType.Warning => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9800")),
                    ToastType.Info => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#685AFF")),
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#685AFF"))
                };
            }
        }

        public event EventHandler? Closed;

        public ToastViewModel(string message, ToastType type = ToastType.Info, int durationMs = 3000)
        {
            Message = message;
            ToastType = type;

            // Tự động đóng sau durationMs
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(durationMs)
            };
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                Close();
            };
            _timer.Start();
        }

        public void Close()
        {
            _timer?.Stop();
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
