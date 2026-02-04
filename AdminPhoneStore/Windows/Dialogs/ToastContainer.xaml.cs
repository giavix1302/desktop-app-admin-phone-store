using System.Windows;
using System.Windows.Controls;
using AdminPhoneStore.ViewModels.Dialogs;
using AdminPhoneStore.Views.Dialogs;

namespace AdminPhoneStore.Windows.Dialogs
{
    /// <summary>
    /// Container window để hiển thị toast notifications
    /// </summary>
    public partial class ToastContainer : Window
    {
        public ToastContainer()
        {
            InitializeComponent();
            
            // Đặt vị trí ở góc dưới bên phải
            UpdatePosition();
            
            // Update position khi screen size thay đổi
            Loaded += (s, e) => UpdatePosition();
        }

        private void UpdatePosition()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            var taskbarHeight = SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height;

            Left = screenWidth - Width - 20;
            Top = screenHeight - Height - taskbarHeight - 20;
        }

        public void ShowToast(ToastViewModel toastViewModel)
        {
            var toastView = new ToastView
            {
                DataContext = toastViewModel
            };

            toastViewModel.Closed += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ToastItemsControl.Items.Remove(toastView);
                    
                    // Đóng window nếu không còn toast nào
                    if (ToastItemsControl.Items.Count == 0)
                    {
                        Hide();
                    }
                    else
                    {
                        UpdatePosition();
                    }
                });
            };

            ToastItemsControl.Items.Add(toastView);
            Show();
            UpdatePosition();
        }
    }
}
