using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;
using System.Net.Http;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Implementation của IBrandService
    /// </summary>
    public class BrandService : IBrandService
    {
        private readonly IApiClientService _apiClient;

        public BrandService(IApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<List<Brand>> GetAllBrandsAsync()
        {
            try
            {
                var brands = await _apiClient.GetAsync<List<Brand>>("brands");
                return brands ?? new List<Brand>();
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Brand?> GetBrandByIdAsync(long id)
        {
            try
            {
                return await _apiClient.GetAsync<Brand>($"brands/{id}");
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Brand?> CreateBrandAsync(CreateBrandRequest request)
        {
            try
            {
                return await _apiClient.PostAsync<CreateBrandRequest, Brand>("brands", request);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Brand?> UpdateBrandAsync(long id, UpdateBrandRequest request)
        {
            try
            {
                return await _apiClient.PutAsync<UpdateBrandRequest, Brand>($"brands/{id}", request);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<bool> DeleteBrandAsync(long id)
        {
            try
            {
                return await _apiClient.DeleteAsync($"brands/{id}");
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Brand?> UploadImageAsync(long id, string filePath)
        {
            try
            {
                var content = new MultipartFormDataContent();
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var fileName = System.IO.Path.GetFileName(filePath);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetMimeType(fileName));
                content.Add(fileContent, "image", fileName);
                return await _apiClient.PutMultipartAsync<Brand>($"brands/{id}/image", content);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(long id)
        {
            try
            {
                return await _apiClient.DeleteAsync($"brands/{id}/image");
            }
            catch (ApiException)
            {
                throw;
            }
        }

        private static string GetMimeType(string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
