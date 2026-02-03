using AdminPhoneStore.Services.Interfaces;
using System.Windows;

namespace AdminPhoneStore.Services.Implementations
{
    /// <summary>
    /// Implementation của IDialogService sử dụng MessageBox
    /// Có thể thay thế bằng custom dialog sau này
    /// </summary>
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "Thông báo")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool ShowConfirmation(string message, string title = "Xác nhận")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public void ShowError(string message, string title = "Lỗi")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
