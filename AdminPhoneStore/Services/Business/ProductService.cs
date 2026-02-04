using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;
using AdminPhoneStore.Services.Business;

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
                // Giả sử API endpoint là: GET /api/products
                var products = await _apiClient.GetAsync<List<Product>>("products");
                return products ?? new List<Product>();
            }
            catch (ApiException)
            {
                // Re-throw để ViewModel có thể handle
                throw;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                // Giả sử API endpoint là: GET /api/products/{id}
                return await _apiClient.GetAsync<Product>($"products/{id}");
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Product?> CreateProductAsync(Product product)
        {
            try
            {
                // Giả sử API endpoint là: POST /api/products
                return await _apiClient.PostAsync<Product, Product>("products", product);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            try
            {
                // Giả sử API endpoint là: PUT /api/products/{id}
                return await _apiClient.PutAsync<Product, Product>($"products/{id}", product);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                // Giả sử API endpoint là: DELETE /api/products/{id}
                return await _apiClient.DeleteAsync($"products/{id}");
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                // Giả sử API endpoint là: GET /api/products/search?q={searchTerm}
                var queryParams = new Dictionary<string, string>
                {
                    { "q", searchTerm }
                };
                
                var products = await _apiClient.GetAsync<List<Product>>("products/search", queryParams);
                return products ?? new List<Product>();
            }
            catch (ApiException)
            {
                throw;
            }
        }
    }
}
