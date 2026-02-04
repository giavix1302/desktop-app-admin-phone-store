using AdminPhoneStore.Services.UI;
using AdminPhoneStore.ViewModels.Dialogs;
using AdminPhoneStore.Windows.Dialogs;
using System.Windows;

namespace AdminPhoneStore.Services.UI
{
    /// <summary>
    /// Implementation của IDialogService sử dụng custom Toast và ConfirmDialog
    /// </summary>
    public class DialogService : IDialogService
    {
        private ToastContainer? _toastContainer;

        public void ShowToast(string message, ToastType type = ToastType.Info, int durationMs = 3000)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Tạo hoặc lấy ToastContainer
                if (_toastContainer == null || !_toastContainer.IsVisible)
                {
                    _toastContainer = new ToastContainer();
                }

                var toastViewModel = new ToastViewModel(message, type, durationMs);
                _toastContainer.ShowToast(toastViewModel);
            });
        }

        public void ShowMessage(string message, string title = "Thông báo")
        {
            ShowToast(message, ToastType.Info);
        }

        public bool ShowConfirmation(string message, string title = "Xác nhận", string confirmText = "Xác nhận", string cancelText = "Hủy")
        {
            bool result = false;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var viewModel = new ConfirmDialogViewModel(message, title, confirmText, cancelText);
                var dialog = new ConfirmDialogWindow(viewModel);
                
                result = dialog.ShowDialog() == true && dialog.Result;
            });

            return result;
        }

        public void ShowError(string message, string title = "Lỗi")
        {
            ShowToast(message, ToastType.Error, 4000); // Error hiển thị lâu hơn
        }

        public void ShowSuccess(string message)
        {
            ShowToast(message, ToastType.Success);
        }
    }
}
