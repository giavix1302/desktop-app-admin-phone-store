namespace AdminPhoneStore.Services.UI
{
    /// <summary>
    /// Service để hiển thị dialogs và toast notifications (thay thế MessageBox)
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Hiển thị toast notification (tự động biến mất)
        /// </summary>
        void ShowToast(string message, ToastType type = ToastType.Info, int durationMs = 3000);

        /// <summary>
        /// Hiển thị message toast (info)
        /// </summary>
        void ShowMessage(string message, string title = "Thông báo");

        /// <summary>
        /// Hiển thị confirmation dialog và trả về kết quả
        /// </summary>
        bool ShowConfirmation(string message, string title = "Xác nhận", string confirmText = "Xác nhận", string cancelText = "Hủy");

        /// <summary>
        /// Hiển thị error toast
        /// </summary>
        void ShowError(string message, string title = "Lỗi");

        /// <summary>
        /// Hiển thị success toast
        /// </summary>
        void ShowSuccess(string message);
    }

    /// <summary>
    /// Loại toast notification
    /// </summary>
    public enum ToastType
    {
        Info,
        Success,
        Warning,
        Error
    }
}
