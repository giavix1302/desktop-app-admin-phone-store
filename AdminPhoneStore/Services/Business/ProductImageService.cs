using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;
using AdminPhoneStore.Services.Auth;
using AdminPhoneStore.Services.Infrastructure;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Implementation của IProductImageService sử dụng HttpClient để xử lý multipart/form-data
    /// </summary>
    public class ProductImageService : IProductImageService
    {
        private readonly HttpClient _httpClient;
        private readonly IApiConfiguration _apiConfiguration;
        private readonly IAuthenticationService? _authenticationService;
        private readonly ILoggerService? _loggerService;

        public ProductImageService(
            HttpClient httpClient,
            IApiConfiguration apiConfiguration,
            IAuthenticationService? authenticationService = null,
            ILoggerService? loggerService = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiConfiguration = apiConfiguration ?? throw new ArgumentNullException(nameof(apiConfiguration));
            _authenticationService = authenticationService;
            _loggerService = loggerService;

            // Setup HttpClient
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(_apiConfiguration.BaseUrl);
            }
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<ProductImage>> GetProductImagesAsync(long productId)
        {
            try
            {
                SyncToken();
                var endpoint = $"products/{productId}/images";
                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<ProductImage>>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                    {
                        return apiResponse.Data;
                    }
                }

                await HandleErrorResponse(response);
                return new List<ProductImage>();
            }
            catch (Exception ex)
            {
                _loggerService?.LogError($"Error getting product images: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<ProductImage> UploadProductImageAsync(long productId, string imageFilePath, string? altText = null, bool isPrimary = false)
        {
            try
            {
                if (!File.Exists(imageFilePath))
                {
                    throw new FileNotFoundException($"Image file not found: {imageFilePath}");
                }

                SyncToken();
                var endpoint = $"products/{productId}/images";

                using var content = new MultipartFormDataContent();

                // Add image file
                var fileBytes = await File.ReadAllBytesAsync(imageFilePath);
                var fileName = Path.GetFileName(imageFilePath);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(fileContent, "image", fileName);

                // Add altText if provided
                if (!string.IsNullOrEmpty(altText))
                {
                    content.Add(new StringContent(altText), "altText");
                }

                // Add isPrimary
                content.Add(new StringContent(isPrimary.ToString().ToLower()), "isPrimary");

                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProductImage>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                    {
                        return apiResponse.Data;
                    }
                }

                await HandleErrorResponse(response);
                throw new Exception("Failed to upload product image");
            }
            catch (Exception ex)
            {
                _loggerService?.LogError($"Error uploading product image: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<ProductImage> UpdateProductImageAsync(long productId, long imageId, string? imageFilePath = null, string? altText = null, bool? isPrimary = null)
        {
            try
            {
                SyncToken();
                var endpoint = $"products/{productId}/images/{imageId}";

                using var content = new MultipartFormDataContent();

                // Add image file if provided
                if (!string.IsNullOrEmpty(imageFilePath) && File.Exists(imageFilePath))
                {
                    var fileBytes = await File.ReadAllBytesAsync(imageFilePath);
                    var fileName = Path.GetFileName(imageFilePath);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    content.Add(fileContent, "image", fileName);
                }

                // Add altText if provided
                if (altText != null)
                {
                    content.Add(new StringContent(altText), "altText");
                }

                // Add isPrimary if provided
                if (isPrimary.HasValue)
                {
                    content.Add(new StringContent(isPrimary.Value.ToString().ToLower()), "isPrimary");
                }

                var response = await _httpClient.PutAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProductImage>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                    {
                        return apiResponse.Data;
                    }
                }

                await HandleErrorResponse(response);
                throw new Exception("Failed to update product image");
            }
            catch (Exception ex)
            {
                _loggerService?.LogError($"Error updating product image: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<bool> DeleteProductImageAsync(long productId, long imageId)
        {
            try
            {
                SyncToken();
                var endpoint = $"products/{productId}/images/{imageId}";
                var response = await _httpClient.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
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

                    return true;
                }

                await HandleErrorResponse(response);
                return false;
            }
            catch (Exception ex)
            {
                _loggerService?.LogError($"Error deleting product image: {ex.Message}", ex);
                throw;
            }
        }

        private void SyncToken()
        {
            if (_authenticationService != null)
            {
                var token = _authenticationService.CurrentToken;
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
        }

        private async Task HandleErrorResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse != null)
                {
                    throw new ApiException($"API Error: {apiResponse.Message}", response.StatusCode);
                }
            }
            catch (JsonException)
            {
                // Not an ApiResponse format
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Unauthorized: Please login again");
            }

            throw new ApiException($"HTTP Error: {response.StatusCode}", response.StatusCode);
        }
    }
}
