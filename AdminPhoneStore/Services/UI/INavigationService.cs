using System.Windows.Controls;

namespace AdminPhoneStore.Services.UI
{
    /// <summary>
    /// Service để quản lý navigation giữa các Views
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// View hiện tại đang được hiển thị
        /// </summary>
        UserControl? CurrentView { get; }

        /// <summary>
        /// Navigate đến một View bằng type
        /// </summary>
        void NavigateTo<T>() where T : UserControl;

        /// <summary>
        /// Navigate đến một View bằng type với parameter
        /// </summary>
        void NavigateTo<T>(object? parameter) where T : UserControl;

        /// <summary>
        /// Navigate về View trước đó (nếu có)
        /// </summary>
        bool CanGoBack { get; }
        void GoBack();

        /// <summary>
        /// Event được raise khi View thay đổi
        /// </summary>
        event EventHandler? ViewChanged;
    }
}
