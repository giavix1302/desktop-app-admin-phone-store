using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Auth;
using AdminPhoneStore.Services.Infrastructure;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AdminPhoneStore.Services.Api
{
    /// <summary>
    /// Implementation của IApiClientService sử dụng HttpClient với retry logic, logging, và auto-refresh token
    /// </summary>
    public class ApiClientService : IApiClientService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILoggerService? _loggerService;
        private readonly IAuthenticationService? _authenticationService;
        private string? _authToken;
        private const int MaxRetryAttempts = 3;
        private const int RetryDelayMs = 1000;
        private bool _isRefreshingToken = false;
        private readonly object _refreshLock = new object();

        public ApiClientService(
            HttpClient httpClient,
            IApiConfiguration apiConfiguration,
            ILoggerService? loggerService = null,
            IAuthenticationService? authenticationService = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = apiConfiguration?.BaseUrl ?? throw new ArgumentNullException(nameof(apiConfiguration));
            _loggerService = loggerService;
            _authenticationService = authenticationService;

            // Setup HttpClient (don't set BaseAddress or Timeout on shared instance)
            // Note: BaseAddress and Timeout cannot be set after HttpClient has been used
            // We'll build full URLs in BuildUrl method instead
            // Timeout and Accept header should be set when creating HttpClient in DI container
        }

        public async Task<T?> GetAsync<T>(string endpoint) where T : class
        {
            return await GetAsync<T>(endpoint, null);
        }

        public async Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null) where T : class
        {
            SyncTokenFromAuthService(); // Sync token trước mỗi request
            var url = BuildUrl(endpoint, queryParams);
            _loggerService?.LogInformation($"API GET: {url}");

            return await ExecuteWithRetryAndAutoRefreshAsync(async () =>
            {
                var response = await _httpClient.GetAsync(url);
                return await HandleResponseAsync<T>(response, url);
            }, url);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
            where TRequest : class
            where TResponse : class
        {
            SyncTokenFromAuthService(); // Sync token trước mỗi request
            var url = BuildUrl(endpoint, null);
            _loggerService?.LogInformation($"API POST: {url}");

            return await ExecuteWithRetryAndAutoRefreshAsync(async () =>
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                return await HandleResponseAsync<TResponse>(response, url);
            }, url);
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
            where TRequest : class
            where TResponse : class
        {
            SyncTokenFromAuthService(); // Sync token trước mỗi request
            var url = BuildUrl(endpoint, null);
            _loggerService?.LogInformation($"API PUT: {url}");

            return await ExecuteWithRetryAndAutoRefreshAsync(async () =>
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);
                return await HandleResponseAsync<TResponse>(response, url);
            }, url);
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            SyncTokenFromAuthService(); // Sync token trước mỗi request
            var url = BuildUrl(endpoint, null);
            _loggerService?.LogInformation($"API DELETE: {url}");

            return await ExecuteWithRetryAndAutoRefreshAsync(async () =>
            {
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Try to parse as ApiResponse for consistency
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        try
                        {
                            var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            if (apiResponse != null && !apiResponse.Success)
                            {
                                throw new ApiException($"API Error: {apiResponse.Message}", response.StatusCode);
                            }
                        }
                        catch (JsonException)
                        {
                            // Not an ApiResponse, that's OK for DELETE
                        }
                    }

                    _loggerService?.LogInformation($"API DELETE Success: {url}");
                    return true;
                }

                await HandleErrorResponse(response);
                return false;
            }, url);
        }

        public void SetAuthToken(string? token)
        {
            _authToken = token;

            if (string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        /// <summary>
        /// Sync token từ AuthenticationService (gọi trước mỗi request nếu cần)
        /// </summary>
        private void SyncTokenFromAuthService()
        {
            if (_authenticationService != null)
            {
                var currentToken = _authenticationService.CurrentToken;
                if (currentToken != _authToken)
                {
                    SetAuthToken(currentToken);
                }
            }
        }

        /// <summary>
        /// Build full URL from endpoint, handling base URL and API prefix correctly
        /// </summary>
        private string BuildUrl(string endpoint, Dictionary<string, string>? queryParams)
        {
            // Build query string if params provided
            string? queryString = null;
            if (queryParams != null && queryParams.Count > 0)
            {
                queryString = string.Join("&", queryParams.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            }

            // If endpoint is already a full URL, use it as-is
            if (endpoint.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                if (queryString != null)
                {
                    return $"{endpoint}?{queryString}";
                }
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
            var baseUrlNormalized = _baseUrl.TrimEnd('/');
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

            var fullUri = new Uri(fullPath);

            if (queryString == null)
                return fullUri.ToString();

            return $"{fullUri}?{queryString}";
        }

        private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response, string endpoint) where T : class
        {
            var content = await response.Content.ReadAsStringAsync();
            
            // Log response content for debugging
            _loggerService?.LogInformation($"API Response Content (first 500 chars): {content?.Substring(0, Math.Min(500, content?.Length ?? 0))}");

            if (response.IsSuccessStatusCode)
            {
                _loggerService?.LogInformation($"API Success: {endpoint}");

                // Configure JsonSerializerOptions with enum handling
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    // Allow enum to be deserialized as both string and number
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };

                // Try to deserialize as ApiResponse<T> first
                try
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, jsonOptions);

                    if (apiResponse != null)
                    {
                        if (!apiResponse.Success)
                        {
                            var errorMessage = apiResponse.Message ?? "API request failed";
                            throw new ApiException($"API Error: {errorMessage}", response.StatusCode);
                        }

                        if (apiResponse.Data == null)
                        {
                            _loggerService?.LogWarning($"API returned success but data is null. Response: {content}");
                            throw new ApiException("API returned success but data is null", response.StatusCode);
                        }

                        _loggerService?.LogInformation($"Deserialized {typeof(T).Name} successfully. Data type: {apiResponse.Data?.GetType().Name}");
                        return apiResponse.Data;
                    }
                }
                catch (JsonException ex)
                {
                    _loggerService?.LogError($"Failed to deserialize as ApiResponse<T>: {ex.Message}. Content: {content?.Substring(0, Math.Min(200, content?.Length ?? 0))}");
                    // Not an ApiResponse wrapper, try direct deserialize
                }

                // Direct deserialize (for endpoints that don't use ApiResponse wrapper)
                try
                {
                    var directResult = JsonSerializer.Deserialize<T>(content, jsonOptions);

                    if (directResult == null)
                    {
                        _loggerService?.LogError($"Failed to deserialize response. Content: {content?.Substring(0, Math.Min(200, content?.Length ?? 0))}");
                        throw new ApiException("Failed to deserialize response", response.StatusCode);
                    }

                    _loggerService?.LogInformation($"Deserialized {typeof(T).Name} directly successfully");
                    return directResult;
                }
                catch (JsonException ex)
                {
                    _loggerService?.LogError($"Failed to deserialize response: {ex.Message}. Content: {content?.Substring(0, Math.Min(500, content?.Length ?? 0))}");
                    throw new ApiException($"Failed to deserialize response: {ex.Message}", response.StatusCode);
                }
            }

            // Handle 401 Unauthorized - try to refresh token
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _loggerService?.LogWarning($"Unauthorized (401) for {endpoint}, attempting token refresh");
                throw new UnauthorizedException("Unauthorized - token may be expired");
            }

            // Handle other errors
            await HandleErrorResponse(response);
            return null!;
        }

        private async Task<T> ExecuteWithRetryAndAutoRefreshAsync<T>(Func<Task<T>> operation, string endpoint)
        {
            int attempt = 0;
            Exception? lastException = null;

            while (attempt < MaxRetryAttempts)
            {
                try
                {
                    return await operation();
                }
                catch (UnauthorizedException) when (attempt < MaxRetryAttempts - 1 && _authenticationService != null)
                {
                    // Try to refresh token
                    attempt++;
                    _loggerService?.LogInformation($"Attempting to refresh token (attempt {attempt}/{MaxRetryAttempts})");

                    lock (_refreshLock)
                    {
                        if (_isRefreshingToken)
                        {
                            // Another thread is refreshing, wait a bit
                            Thread.Sleep(500);
                            continue;
                        }
                        _isRefreshingToken = true;
                    }

                    try
                    {
                        var refreshed = await _authenticationService.RefreshTokenAsync();
                        if (refreshed)
                        {
                            _loggerService?.LogInformation("Token refreshed successfully, retrying request");
                            // Update token in this service từ AuthenticationService
                            var newToken = _authenticationService.CurrentToken;
                            if (newToken != null)
                            {
                                SetAuthToken(newToken);
                            }
                            _isRefreshingToken = false;
                            // Retry the operation
                            continue;
                        }
                        else
                        {
                            _isRefreshingToken = false;
                            throw new ApiException("Token refresh failed", System.Net.HttpStatusCode.Unauthorized);
                        }
                    }
                    catch (Exception ex)
                    {
                        _isRefreshingToken = false;
                        _loggerService?.LogError("Token refresh failed", ex);
                        throw new ApiException("Token refresh failed", System.Net.HttpStatusCode.Unauthorized);
                    }
                }
                catch (HttpRequestException ex) when (attempt < MaxRetryAttempts - 1)
                {
                    attempt++;
                    lastException = ex;
                    _loggerService?.LogWarning($"API Request failed (attempt {attempt}/{MaxRetryAttempts}): {endpoint}. Retrying...");
                    await Task.Delay(RetryDelayMs * attempt);
                }
                catch (TaskCanceledException ex) when (attempt < MaxRetryAttempts - 1)
                {
                    attempt++;
                    lastException = ex;
                    _loggerService?.LogWarning($"API Request timeout (attempt {attempt}/{MaxRetryAttempts}): {endpoint}. Retrying...");
                    await Task.Delay(RetryDelayMs * attempt);
                }
                catch (Exception ex)
                {
                    _loggerService?.LogError($"API Error: {endpoint}", ex);
                    throw;
                }
            }

            // All retries failed
            _loggerService?.LogError($"API Request failed after {MaxRetryAttempts} attempts: {endpoint}", lastException);

            if (lastException is HttpRequestException httpEx)
            {
                throw new ApiException($"Lỗi khi gọi API sau {MaxRetryAttempts} lần thử: {httpEx.Message}", httpEx);
            }
            else if (lastException is TaskCanceledException timeoutEx)
            {
                throw new ApiException($"Request timeout sau {MaxRetryAttempts} lần thử. Vui lòng kiểm tra kết nối mạng.", timeoutEx);
            }

            throw new ApiException($"Lỗi không xác định sau {MaxRetryAttempts} lần thử", lastException);
        }

        private async Task HandleErrorResponse(HttpResponseMessage response)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var statusCode = response.StatusCode;

            // Try to parse as ApiResponse for error message
            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.Message))
                {
                    _loggerService?.LogError($"API Error Response: {statusCode} - {apiResponse.Message}");
                    throw new ApiException($"API Error: {apiResponse.Message}", statusCode);
                }
            }
            catch (JsonException)
            {
                // Not an ApiResponse, use raw content
            }

            _loggerService?.LogError($"API Error Response: {statusCode} - {errorContent}");
            throw new ApiException($"API Error: {statusCode} - {errorContent}", statusCode);
        }
    }

    /// <summary>
    /// Custom exception cho Unauthorized (401) để trigger token refresh
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }

    /// <summary>
    /// Custom exception cho API errors
    /// </summary>
    public class ApiException : Exception
    {
        public System.Net.HttpStatusCode? StatusCode { get; }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, Exception innerException)
            : base(message, innerException) { }

        public ApiException(string message, System.Net.HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
