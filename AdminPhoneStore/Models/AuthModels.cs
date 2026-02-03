namespace AdminPhoneStore.Models
{
    /// <summary>
    /// Request model cho login
    /// </summary>
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// User information từ API
    /// </summary>
    public class UserInfo
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    /// <summary>
    /// Data trong LoginResponse
    /// </summary>
    public class LoginResponseData
    {
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public UserInfo? User { get; set; }
    }

    /// <summary>
    /// Request model cho refresh token (admin)
    /// </summary>
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data trong RefreshTokenResponse
    /// </summary>
    public class RefreshTokenResponseData
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
