using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;

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
    }
}
