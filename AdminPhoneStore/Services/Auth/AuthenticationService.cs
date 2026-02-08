using AdminPhoneStore.Services.Auth;
using AdminPhoneStore.Models;
using AdminPhoneStore.Helpers;
using AdminPhoneStore.Services.Infrastructure;
using AdminPhoneStore.Services.Api;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AdminPhoneStore.Services.Auth
{
    /// <summary>
    /// Implementation của IAuthenticationService theo API docs
    /// Sử dụng HttpClient trực tiếp để tránh circular dependency với ApiClientService
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly IApiConfiguration _apiConfiguration;
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
            HttpClient httpClient,
            IApiConfiguration apiConfiguration,
            ITokenStorageService tokenStorage,
            ILoggerService? loggerService = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiConfiguration = apiConfiguration ?? throw new ArgumentNullException(nameof(apiConfiguration));
            _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
            _loggerService = loggerService;

            // Setup HttpClient cho auth endpoints
            // Note: Don't set BaseAddress on shared HttpClient instance
            // We'll build full URLs in each request instead
            // Only set Accept header if not already set
            if (!_httpClient.DefaultRequestHeaders.Accept.Any())
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
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
                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = BuildUrl("auth/login");
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseData>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                    {
                        var data = apiResponse.Data;
                        
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

                        _loggerService?.LogInformation($"Login successful for email: {email}");
                        OnAuthenticationStateChanged(true);
                        return true;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _loggerService?.LogWarning($"Login failed: {response.StatusCode} - {errorContent}");
                }

                _loggerService?.LogWarning($"Login failed for email: {email}");
                return false;
            }
            catch (Exception ex)
            {
                _loggerService?.LogError($"Login error for email: {email}", ex);
                throw new ApiException($"Lỗi khi đăng nhập: {ex.Message}", ex);
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
                        var url = BuildUrl("auth/logout");
                        var request = new HttpRequestMessage(HttpMethod.Post, url);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _currentAccessToken);
                        var response = await _httpClient.SendAsync(request);
                        
                        if (!response.IsSuccessStatusCode)
                        {
                            _loggerService?.LogWarning($"Logout API call failed: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _loggerService?.LogWarning($"Logout API call failed: {ex.Message}");
                        // Continue với logout local dù API call thất bại
                    }
                }

                // Clear local state
                _currentAccessToken = null;
                _currentUser = null;
                _tokenStorage.ClearTokens();

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

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = BuildUrl("auth/refresh-admin");
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<RefreshTokenResponseData>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                    {
                        var data = apiResponse.Data;
                        
                        // Lưu tokens mới vào storage
                        _tokenStorage.SaveTokens(data.AccessToken, data.RefreshToken);
                        
                        // Update current token
                        _currentAccessToken = data.AccessToken;
                        
                        _loggerService?.LogInformation("Token refreshed successfully");
                        return true;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _loggerService?.LogWarning($"Token refresh failed: {response.StatusCode} - {errorContent}");
                    
                    // Nếu 401 Unauthorized, refreshToken đã hết hạn -> logout
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _loggerService?.LogWarning("Refresh token expired, logging out");
                        await LogoutAsync();
                    }
                }

                _loggerService?.LogWarning("Token refresh failed: Invalid response");
                await LogoutAsync();
                return false;
            }
            catch (Exception ex)
            {
                _loggerService?.LogError("Token refresh failed", ex);
                await LogoutAsync();
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

        /// <summary>
        /// Build full URL from endpoint, handling base URL and API prefix correctly
        /// </summary>
        private string BuildUrl(string endpoint)
        {
            // If endpoint is already a full URL, use it as-is
            if (endpoint.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return endpoint;
            }

            // Normalize endpoint - remove leading slashes
            var normalizedEndpoint = endpoint.TrimStart('/');
            
            // Remove "api/" prefix from endpoint if it exists (we'll handle it based on BaseUrl)
            if (normalizedEndpoint.StartsWith("api/", StringComparison.OrdinalIgnoreCase))
            {
                normalizedEndpoint = normalizedEndpoint.Substring(4); // Remove "api/"
            }
            
            // Check if base URL already contains "/api"
            var baseUrlNormalized = _apiConfiguration.BaseUrl.TrimEnd('/');
            var baseUrlLower = baseUrlNormalized.ToLowerInvariant();
            var baseUrlHasApi = baseUrlLower.EndsWith("/api");
            
            // Build the full path
            string fullPath;
            if (baseUrlHasApi)
            {
                // BaseUrl already has /api, just append endpoint
                fullPath = $"{baseUrlNormalized}/{normalizedEndpoint}";
            }
            else
            {
                // BaseUrl doesn't have /api, add it
                fullPath = $"{baseUrlNormalized}/api/{normalizedEndpoint}";
            }

            return new Uri(fullPath).ToString();
        }
    }
}
