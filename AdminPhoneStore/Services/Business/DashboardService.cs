using AdminPhoneStore.Models.Dashboard;
using AdminPhoneStore.Services.Api;

namespace AdminPhoneStore.Services.Business
{
    public class DashboardService : IDashboardService
    {
        private readonly IApiClientService _apiClient;

        public DashboardService(IApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<DashboardSummary?> GetSummaryAsync()
        {
            try
            {
                return await _apiClient.GetAsync<DashboardSummary>("admin/dashboard/summary");
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<RevenueChartData?> GetRevenueChartAsync(string period)
        {
            try
            {
                var queryParams = new Dictionary<string, string> { ["period"] = period };
                return await _apiClient.GetAsync<RevenueChartData>("admin/dashboard/revenue-chart", queryParams);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<OrderStatusChartData?> GetOrderStatusChartAsync()
        {
            try
            {
                return await _apiClient.GetAsync<OrderStatusChartData>("admin/dashboard/order-status-chart");
            }
            catch (ApiException)
            {
                throw;
            }
        }
    }
}
