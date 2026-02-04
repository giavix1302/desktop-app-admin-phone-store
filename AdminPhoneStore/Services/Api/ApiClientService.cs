using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;
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

            // Setup HttpClient
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(apiConfiguration.TimeoutSeconds);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
            _loggerService?.LogInformation($"API POST: {endpoint}");

            return await ExecuteWithRetryAndAutoRefreshAsync(async () =>
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content);
                return await HandleResponseAsync<TResponse>(response, endpoint);
            }, endpoint);
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
            where TRequest : class
            where TResponse : class
        {
            SyncTokenFromAuthService(); // Sync token trước mỗi request
            _loggerService?.LogInformation($"API PUT: {endpoint}");

            return await ExecuteWithRetryAndAutoRefreshAsync(async () =>
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(endpoint, content);
                return await HandleResponseAsync<TResponse>(response, endpoint);
            }, endpoint);
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            SyncTokenFromAuthService(); // Sync token trước mỗi request
            _loggerService?.LogInformation($"API DELETE: {endpoint}");

            return await ExecuteWithRetryAndAutoRefreshAsync(async () =>
            {
                var response = await _httpClient.DeleteAsync(endpoint);

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

                    _loggerService?.LogInformation($"API DELETE Success: {endpoint}");
                    return true;
                }

                await HandleErrorResponse(response);
                return false;
            }, endpoint);
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

        private string BuildUrl(string endpoint, Dictionary<string, string>? queryParams)
        {
            if (queryParams == null || queryParams.Count == 0)
                return endpoint;

            var queryString = string.Join("&", queryParams.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            return $"{endpoint}?{queryString}";
        }

        private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response, string endpoint) where T : class
        {
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _loggerService?.LogInformation($"API Success: {endpoint}");

                // Try to deserialize as ApiResponse<T> first
                try
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse != null)
                    {
                        if (!apiResponse.Success)
                        {
                            var errorMessage = apiResponse.Message ?? "API request failed";
                            throw new ApiException($"API Error: {errorMessage}", response.StatusCode);
                        }

                        if (apiResponse.Data == null)
                        {
                            throw new ApiException("API returned success but data is null", response.StatusCode);
                        }

                        return apiResponse.Data;
                    }
                }
                catch (JsonException)
                {
                    // Not an ApiResponse wrapper, try direct deserialize
                }

                // Direct deserialize (for endpoints that don't use ApiResponse wrapper)
                var directResult = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (directResult == null)
                {
                    throw new ApiException("Failed to deserialize response", response.StatusCode);
                }

                return directResult;
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
