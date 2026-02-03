using AdminPhoneStore.Models;

namespace AdminPhoneStore.Services.Interfaces
{
    /// <summary>
    /// Service để quản lý authentication (login, logout, token management)
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Kiểm tra xem user đã đăng nhập chưa
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Access token hiện tại (nếu có)
        /// </summary>
        string? CurrentToken { get; }

        /// <summary>
        /// Thông tin user hiện tại
        /// </summary>
        UserInfo? CurrentUser { get; }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        Task<bool> LoginAsync(string email, string password);

        /// <summary>
        /// Đăng xuất
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Refresh token (refresh-admin endpoint)
        /// </summary>
        Task<bool> RefreshTokenAsync();

        /// <summary>
        /// Load tokens từ storage khi app khởi động
        /// </summary>
        Task<bool> LoadTokensFromStorageAsync();

        /// <summary>
        /// Event được raise khi authentication state thay đổi
        /// </summary>
        event EventHandler<bool>? AuthenticationStateChanged;
    }
}
