using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Implementation của IColorService
    /// </summary>
    public class ColorService : IColorService
    {
        private readonly IApiClientService _apiClient;

        public ColorService(IApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<List<Color>> GetAllColorsAsync()
        {
            try
            {
                var colors = await _apiClient.GetAsync<List<Color>>("colors");
                return colors ?? new List<Color>();
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<Color?> GetColorByIdAsync(long id)
        {
            try
            {
                return await _apiClient.GetAsync<Color>($"colors/{id}");
            }
            catch (ApiException)
            {
                throw;
            }
        }
    }
}
