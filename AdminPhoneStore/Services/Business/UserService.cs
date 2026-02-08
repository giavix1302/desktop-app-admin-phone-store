using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Implementation của IUserService
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IApiClientService _apiClient;

        public UserService(IApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<PagedResponse<User>> GetUsersAsync(UserFilterRequest? filter = null)
        {
            try
            {
                var queryParams = new Dictionary<string, string>();

                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(filter.Email))
                        queryParams["email"] = filter.Email;
                    if (!string.IsNullOrEmpty(filter.FullName))
                        queryParams["fullName"] = filter.FullName;
                    if (filter.Enabled.HasValue)
                        queryParams["enabled"] = filter.Enabled.Value.ToString().ToLower();
                    if (filter.From.HasValue)
                        queryParams["from"] = filter.From.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    if (filter.To.HasValue)
                        queryParams["to"] = filter.To.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    if (!string.IsNullOrEmpty(filter.SortBy))
                        queryParams["sortBy"] = filter.SortBy;
                    if (!string.IsNullOrEmpty(filter.SortDir))
                        queryParams["sortDir"] = filter.SortDir;
                    queryParams["page"] = filter.Page.ToString();
                    queryParams["pageSize"] = filter.PageSize.ToString();
                }

                var response = await _apiClient.GetAsync<PagedResponse<User>>("admin/users", queryParams);
                return response ?? new PagedResponse<User>();
            }
            catch (ApiException ex)
            {
                System.Diagnostics.Debug.WriteLine($"UserService.GetUsersAsync: ApiException - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UserService.GetUsersAsync: Exception - {ex.Message}");
                throw;
            }
        }

        public async Task<UserDetail?> GetUserByIdAsync(long userId)
        {
            try
            {
                var response = await _apiClient.GetAsync<UserDetail>($"admin/users/{userId}");
                return response;
            }
            catch (ApiException ex)
            {
                System.Diagnostics.Debug.WriteLine($"UserService.GetUserByIdAsync: ApiException - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UserService.GetUserByIdAsync: Exception - {ex.Message}");
                throw;
            }
        }
    }
}
