namespace AdminPhoneStore.Services.Interfaces
{
    /// <summary>
    /// Service để hiển thị dialogs (thay thế MessageBox trực tiếp)
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Hiển thị message box
        /// </summary>
        void ShowMessage(string message, string title = "Thông báo");

        /// <summary>
        /// Hiển thị message box và trả về kết quả
        /// </summary>
        bool ShowConfirmation(string message, string title = "Xác nhận");

        /// <summary>
        /// Hiển thị error message
        /// </summary>
        void ShowError(string message, string title = "Lỗi");
    }
}
