using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;

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
    }
}
