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

        public async Task<ProductPagedResponse> GetAllProductsAsync(ProductFilterRequest? filter = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (filter != null)
            {
                if (filter.BrandId.HasValue)
                    queryParams["brandId"] = filter.BrandId.Value.ToString();
                if (filter.CategoryId.HasValue)
                    queryParams["categoryId"] = filter.CategoryId.Value.ToString();
                if (filter.MinPrice.HasValue)
                    queryParams["minPrice"] = filter.MinPrice.Value.ToString();
                if (filter.MaxPrice.HasValue)
                    queryParams["maxPrice"] = filter.MaxPrice.Value.ToString();
                if (!string.IsNullOrEmpty(filter.SortBy))
                    queryParams["sortBy"] = filter.SortBy;
                if (!string.IsNullOrEmpty(filter.SortDir))
                    queryParams["sortDir"] = filter.SortDir;
                queryParams["page"] = filter.Page.ToString();
                queryParams["pageSize"] = filter.PageSize.ToString();
                if (!string.IsNullOrEmpty(filter.Search))
                    queryParams["search"] = filter.Search;
            }

            // HandleResponseAsync tự unwrap ApiResponse<T>, nên truyền T = ProductPagedResponse trực tiếp
            return await _apiClient.GetAsync<ProductPagedResponse>("/api/products", queryParams)
                   ?? new ProductPagedResponse();
        }

        public async Task<Product?> GetProductByIdAsync(long id)
        {
            return await _apiClient.GetAsync<Product>($"/api/products/{id}");
        }

        public async Task<Product?> CreateProductAsync(CreateProductRequest request)
        {
            return await _apiClient.PostAsync<CreateProductRequest, Product>("/api/products", request);
        }

        public async Task<Product?> UpdateProductAsync(long id, UpdateProductRequest request)
        {
            return await _apiClient.PutAsync<UpdateProductRequest, Product>($"/api/products/{id}", request);
        }

        public async Task<bool> DeleteProductAsync(long id)
        {
            return await _apiClient.DeleteAsync($"/api/products/{id}");
        }
    }
}
