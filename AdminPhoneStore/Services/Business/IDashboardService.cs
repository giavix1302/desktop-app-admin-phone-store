using AdminPhoneStore.Models.Dashboard;

namespace AdminPhoneStore.Services.Business
{
    public interface IDashboardService
    {
        Task<DashboardSummary?> GetSummaryAsync();
        Task<RevenueChartData?> GetRevenueChartAsync(string period);
        Task<OrderStatusChartData?> GetOrderStatusChartAsync();
    }
}
