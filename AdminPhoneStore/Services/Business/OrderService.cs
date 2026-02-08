using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;
using System.Text.Json;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Implementation của IOrderService
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IApiClientService _apiClient;

        public OrderService(IApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<PagedResponse<Order>> GetOrdersAsync(OrderFilterRequest? filter = null)
        {
            try
            {
                var queryParams = new Dictionary<string, string>();

                if (filter != null)
                {
                    if (filter.Status.HasValue)
                        queryParams["status"] = filter.Status.Value.ToString();
                    if (filter.PaymentStatus.HasValue)
                        queryParams["paymentStatus"] = filter.PaymentStatus.Value.ToString();
                    if (filter.PaymentMethod.HasValue)
                        queryParams["paymentMethod"] = filter.PaymentMethod.Value.ToString();
                    if (filter.UserId.HasValue)
                        queryParams["userId"] = filter.UserId.Value.ToString();
                    if (!string.IsNullOrEmpty(filter.OrderNumber))
                        queryParams["orderNumber"] = filter.OrderNumber;
                    if (filter.From.HasValue)
                        queryParams["from"] = filter.From.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    if (filter.To.HasValue)
                        queryParams["to"] = filter.To.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    if (filter.MinTotal.HasValue)
                        queryParams["minTotal"] = filter.MinTotal.Value.ToString();
                    if (filter.MaxTotal.HasValue)
                        queryParams["maxTotal"] = filter.MaxTotal.Value.ToString();
                    if (!string.IsNullOrEmpty(filter.SortBy))
                        queryParams["sortBy"] = filter.SortBy;
                    if (!string.IsNullOrEmpty(filter.SortDir))
                        queryParams["sortDir"] = filter.SortDir;
                    queryParams["page"] = filter.Page.ToString();
                    queryParams["pageSize"] = filter.PageSize.ToString();
                }

                System.Diagnostics.Debug.WriteLine($"OrderService.GetOrdersAsync: Calling API with {queryParams.Count} query params");
                var response = await _apiClient.GetAsync<PagedResponse<Order>>("admin/orders", queryParams);
                
                System.Diagnostics.Debug.WriteLine($"OrderService.GetOrdersAsync: Response received. IsNull: {response == null}");
                if (response != null)
                {
                    System.Diagnostics.Debug.WriteLine($"OrderService.GetOrdersAsync: TotalItems: {response.TotalItems}, Items count: {response.Items?.Count ?? 0}");
                    if (response.Items != null && response.Items.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"OrderService.GetOrdersAsync: First order ID: {response.Items[0].Id}, OrderNumber: {response.Items[0].OrderNumber}");
                    }
                }
                
                return response ?? new PagedResponse<Order>();
            }
            catch (ApiException ex)
            {
                System.Diagnostics.Debug.WriteLine($"OrderService.GetOrdersAsync: ApiException - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OrderService.GetOrdersAsync: Exception - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<OrderDetail?> GetOrderByIdAsync(long orderId)
        {
            try
            {
                return await _apiClient.GetAsync<OrderDetail>($"admin/orders/{orderId}");
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(long orderId, UpdateOrderStatusRequest request)
        {
            try
            {
                var response = await _apiClient.PutAsync<UpdateOrderStatusRequest, object>(
                    $"admin/orders/{orderId}/status", request);
                return response != null;
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<bool> UpdateOrderPaymentAsync(long orderId, UpdateOrderPaymentRequest request)
        {
            try
            {
                var response = await _apiClient.PutAsync<UpdateOrderPaymentRequest, object>(
                    $"admin/orders/{orderId}/payment", request);
                return response != null;
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<AddTrackingResponse?> AddTrackingAsync(long orderId, AddTrackingRequest request)
        {
            try
            {
                return await _apiClient.PostAsync<AddTrackingRequest, AddTrackingResponse>(
                    $"admin/orders/{orderId}/tracking", request);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<bool> UpdateTrackingAsync(long orderId, long trackingId, AddTrackingRequest request)
        {
            try
            {
                var response = await _apiClient.PutAsync<AddTrackingRequest, object>(
                    $"admin/orders/{orderId}/tracking/{trackingId}", request);
                return response != null;
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<bool> DeleteTrackingAsync(long orderId, long trackingId)
        {
            try
            {
                return await _apiClient.DeleteAsync($"admin/orders/{orderId}/tracking/{trackingId}");
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<OrderStats?> GetOrderStatsAsync(DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var queryParams = new Dictionary<string, string>();
                if (from.HasValue)
                    queryParams["from"] = from.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
                if (to.HasValue)
                    queryParams["to"] = to.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");

                return await _apiClient.GetAsync<OrderStats>("admin/orders/stats", 
                    queryParams.Count > 0 ? queryParams : null);
            }
            catch (ApiException)
            {
                throw;
            }
        }
    }
}
