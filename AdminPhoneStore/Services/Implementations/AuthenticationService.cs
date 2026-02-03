using AdminPhoneStore.Services.Interfaces;
using AdminPhoneStore.Models;
using AdminPhoneStore.Helpers;

namespace AdminPhoneStore.Services.Implementations
{
    /// <summary>
    /// Implementation của IAuthenticationService theo API docs
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApiClientService _apiClient;
        private readonly ITokenStorageService _tokenStorage;
        private readonly ILoggerService? _loggerService;
        private string? _currentAccessToken;
        private UserInfo? _currentUser;

        public bool IsAuthenticated => !string.IsNullOrEmpty(_currentAccessToken) && 
                                      !JwtHelper.IsTokenExpired(_currentAccessToken);

        public string? CurrentToken => _currentAccessToken;

        public UserInfo? CurrentUser => _currentUser;

        public event EventHandler<bool>? AuthenticationStateChanged;

        public AuthenticationService(
            IApiClientService apiClient, 
            ITokenStorageService tokenStorage,
            ILoggerService? loggerService = null)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
            _loggerService = loggerService;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                _loggerService?.LogInformation($"Attempting login for email: {email}");

                var loginRequest = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                // Gọi API: POST /api/auth/login
                var response = await _apiClient.PostAsync<LoginRequest, ApiResponse<LoginResponseData>>("auth/login", loginRequest);

                if (response != null && response.Success && response.Data != null)
                {
                    var data = response.Data;
                    
                    if (string.IsNullOrEmpty(data.AccessToken))
                    {
                        _loggerService?.LogWarning($"Login failed: No access token in response");
                        return false;
                    }

                    // Admin phải có refreshToken
                    if (string.IsNullOrEmpty(data.RefreshToken))
                    {
                        _loggerService?.LogWarning($"Login failed: No refresh token in response (Admin required)");
                        return false;
                    }

                    // Lưu tokens vào storage
                    _tokenStorage.SaveTokens(data.AccessToken, data.RefreshToken);
                    
                    // Set current token
                    _currentAccessToken = data.AccessToken;
                    _currentUser = data.User;
                    
                    // Set token vào API client
                    _apiClient.SetAuthToken(_currentAccessToken);

                    _loggerService?.LogInformation($"Login successful for email: {email}");
                    OnAuthenticationStateChanged(true);
                    return true;
                }

                _loggerService?.LogWarning($"Login failed for email: {email}");
                return false;
            }
            catch (ApiException ex)
            {
                _loggerService?.LogError($"Login error for email: {email}", ex);
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                _loggerService?.LogInformation("Logging out user");

                // Gọi API logout nếu có token
                if (!string.IsNullOrEmpty(_currentAccessToken))
                {
                    try
                    {
                        await _apiClient.PostAsync<object, ApiResponse<object>>("auth/logout", new { });
                    }
                    catch (ApiException ex)
                    {
                        _loggerService?.LogWarning($"Logout API call failed: {ex.Message}");
                        // Continue với logout local dù API call thất bại
                    }
                }

                // Clear local state
                _currentAccessToken = null;
                _currentUser = null;
                _tokenStorage.ClearTokens();
                _apiClient.SetAuthToken(null);

                _loggerService?.LogInformation("User logged out");
                OnAuthenticationStateChanged(false);
            }
            catch (Exception ex)
            {
                _loggerService?.LogError("Error during logout", ex);
                // Force clear local state even if there's an error
                _currentAccessToken = null;
                _currentUser = null;
                _tokenStorage.ClearTokens();
                _apiClient.SetAuthToken(null);
                OnAuthenticationStateChanged(false);
            }
        }

        public async Task<bool> RefreshTokenAsync()
        {
            try
            {
                var refreshToken = _tokenStorage.GetRefreshToken();
                if (string.IsNullOrEmpty(refreshToken))
                {
                    _loggerService?.LogWarning("Cannot refresh token: No refresh token available");
                    await LogoutAsync();
                    return false;
                }

                _loggerService?.LogInformation("Refreshing token");

                // Gọi API: POST /api/auth/refresh-admin
                var request = new RefreshTokenRequest
                {
                    RefreshToken = refreshToken
                };

                var response = await _apiClient.PostAsync<RefreshTokenRequest, ApiResponse<RefreshTokenResponseData>>(
                    "auth/refresh-admin", 
                    request);

                if (response != null && response.Success && response.Data != null)
                {
                    var data = response.Data;
                    
                    // Lưu tokens mới vào storage
                    _tokenStorage.SaveTokens(data.AccessToken, data.RefreshToken);
                    
                    // Update current token
                    _currentAccessToken = data.AccessToken;
                    
                    // Update API client
                    _apiClient.SetAuthToken(_currentAccessToken);
                    
                    _loggerService?.LogInformation("Token refreshed successfully");
                    return true;
                }

                _loggerService?.LogWarning("Token refresh failed: Invalid response");
                await LogoutAsync();
                return false;
            }
            catch (ApiException ex)
            {
                _loggerService?.LogError("Token refresh failed", ex);
                
                // Nếu 401 Unauthorized, refreshToken đã hết hạn -> logout
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _loggerService?.LogWarning("Refresh token expired, logging out");
                    await LogoutAsync();
                }
                
                return false;
            }
        }

        public async Task<bool> LoadTokensFromStorageAsync()
        {
            try
            {
                if (!_tokenStorage.HasTokens())
                {
                    _loggerService?.LogInformation("No tokens found in storage");
                    return false;
                }

                var accessToken = _tokenStorage.GetAccessToken();
                if (string.IsNullOrEmpty(accessToken))
                {
                    _loggerService?.LogInformation("No access token found in storage");
                    return false;
                }

                // Check token expiration
                if (JwtHelper.IsTokenExpired(accessToken))
                {
                    _loggerService?.LogInformation("Stored access token expired, attempting refresh");
                    
                    // Try to refresh
                    var refreshed = await RefreshTokenAsync();
                    if (!refreshed)
                    {
                        _loggerService?.LogWarning("Failed to refresh expired token");
                        return false;
                    }
                    
                    // Get new token after refresh
                    accessToken = _tokenStorage.GetAccessToken();
                }

                // Set token
                _currentAccessToken = accessToken;
                _apiClient.SetAuthToken(_currentAccessToken);

                // Try to extract user info from token (if available in claims)
                try
                {
                    var claims = JwtHelper.DecodeJwt(accessToken);
                    if (claims.TryGetValue("email", out var email) && 
                        claims.TryGetValue("sub", out var userId))
                    {
                        _currentUser = new UserInfo
                        {
                            Id = int.TryParse(userId, out var id) ? id : 0,
                            Email = email,
                            FullName = claims.TryGetValue("name", out var name) ? name : string.Empty,
                            Roles = claims.TryGetValue("roles", out var roles) 
                                ? roles.Split(',').ToList() 
                                : new List<string>()
                        };
                    }
                }
                catch (Exception ex)
                {
                    _loggerService?.LogWarning($"Could not extract user info from token: {ex.Message}");
                }

                _loggerService?.LogInformation("Tokens loaded from storage successfully");
                OnAuthenticationStateChanged(true);
                return true;
            }
            catch (Exception ex)
            {
                _loggerService?.LogError("Error loading tokens from storage", ex);
                return false;
            }
        }

        private void OnAuthenticationStateChanged(bool isAuthenticated)
        {
            AuthenticationStateChanged?.Invoke(this, isAuthenticated);
        }
    }
}
