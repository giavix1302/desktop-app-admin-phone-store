using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Implementation của IProductService sử dụng API Client
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IApiClientService _apiClient;

        public ProductService(IApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                var products = await _apiClient.GetAsync<List<Product>>("products");
                return products ?? new List<Product>();
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Product?> GetProductByIdAsync(long id)
        {
            try
            {
                return await _apiClient.GetAsync<Product>($"products/{id}");
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Product?> CreateProductAsync(CreateProductRequest request)
        {
            try
            {
                return await _apiClient.PostAsync<CreateProductRequest, Product>("products", request);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Product?> UpdateProductAsync(long id, UpdateProductRequest request)
        {
            try
            {
                return await _apiClient.PutAsync<UpdateProductRequest, Product>($"products/{id}", request);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(long id)
        {
            try
            {
                return await _apiClient.DeleteAsync($"products/{id}");
            }
            catch (ApiException)
            {
                throw;
            }
        }
    }
}
