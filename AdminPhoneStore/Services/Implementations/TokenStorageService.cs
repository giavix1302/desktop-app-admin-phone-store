using AdminPhoneStore.Services.Interfaces;
using AdminPhoneStore.Properties;

namespace AdminPhoneStore.Services.Implementations
{
    /// <summary>
    /// Implementation của ITokenStorageService sử dụng ApplicationSettings (Windows Settings)
    /// </summary>
    public class TokenStorageService : ITokenStorageService
    {
        public void SaveTokens(string accessToken, string refreshToken)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentException("Tokens cannot be null or empty");
            }

            // Lưu vào user settings (persist giữa các lần mở app)
            Settings.Default.AccessToken = accessToken;
            Settings.Default.RefreshToken = refreshToken;
            Settings.Default.Save();
        }

        public string? GetAccessToken()
        {
            return Settings.Default.AccessToken;
        }

        public string? GetRefreshToken()
        {
            return Settings.Default.RefreshToken;
        }

        public void ClearTokens()
        {
            Settings.Default.AccessToken = string.Empty;
            Settings.Default.RefreshToken = string.Empty;
            Settings.Default.Save();
        }

        public bool HasTokens()
        {
            var accessToken = GetAccessToken();
            var refreshToken = GetRefreshToken();
            return !string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken);
        }
    }
}
