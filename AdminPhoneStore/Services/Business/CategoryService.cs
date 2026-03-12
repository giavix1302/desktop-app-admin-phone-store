using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;
using System.Net.Http;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Implementation của ICategoryService
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly IApiClientService _apiClient;

        public CategoryService(IApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _apiClient.GetAsync<List<Category>>("categories");
                return categories ?? new List<Category>();
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(long id)
        {
            try
            {
                return await _apiClient.GetAsync<Category>($"categories/{id}");
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Category?> CreateCategoryAsync(CreateCategoryRequest request)
        {
            try
            {
                return await _apiClient.PostAsync<CreateCategoryRequest, Category>("categories", request);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Category?> UpdateCategoryAsync(long id, UpdateCategoryRequest request)
        {
            try
            {
                return await _apiClient.PutAsync<UpdateCategoryRequest, Category>($"categories/{id}", request);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(long id)
        {
            try
            {
                return await _apiClient.DeleteAsync($"categories/{id}");
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Category?> UploadImageAsync(long id, string filePath)
        {
            try
            {
                var content = new MultipartFormDataContent();
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var fileName = System.IO.Path.GetFileName(filePath);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetMimeType(fileName));
                content.Add(fileContent, "image", fileName);
                return await _apiClient.PutMultipartAsync<Category>($"categories/{id}/image", content);
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
                return await _apiClient.DeleteAsync($"categories/{id}/image");
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
