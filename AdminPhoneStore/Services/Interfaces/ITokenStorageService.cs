namespace AdminPhoneStore.Services.Interfaces
{
    /// <summary>
    /// Service để lưu trữ và quản lý tokens (accessToken, refreshToken)
    /// </summary>
    public interface ITokenStorageService
    {
        /// <summary>
        /// Lưu accessToken và refreshToken
        /// </summary>
        void SaveTokens(string accessToken, string refreshToken);

        /// <summary>
        /// Lấy accessToken
        /// </summary>
        string? GetAccessToken();

        /// <summary>
        /// Lấy refreshToken
        /// </summary>
        string? GetRefreshToken();

        /// <summary>
        /// Xóa tất cả tokens
        /// </summary>
        void ClearTokens();

        /// <summary>
        /// Kiểm tra xem có tokens không
        /// </summary>
        bool HasTokens();
    }
}
