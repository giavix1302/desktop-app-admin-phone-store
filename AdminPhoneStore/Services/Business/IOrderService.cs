using AdminPhoneStore.Models;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Service để quản lý Order (Admin)
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Lấy danh sách orders với filter, search, sort và pagination
        /// </summary>
        Task<PagedResponse<Order>> GetOrdersAsync(OrderFilterRequest? filter = null);

        /// <summary>
        /// Lấy chi tiết order theo ID
        /// </summary>
        Task<OrderDetail?> GetOrderByIdAsync(long orderId);

        /// <summary>
        /// Cập nhật trạng thái order
        /// </summary>
        Task<bool> UpdateOrderStatusAsync(long orderId, UpdateOrderStatusRequest request);

        /// <summary>
        /// Cập nhật thông tin thanh toán order
        /// </summary>
        Task<bool> UpdateOrderPaymentAsync(long orderId, UpdateOrderPaymentRequest request);

        /// <summary>
        /// Thêm tracking cho order
        /// </summary>
        Task<AddTrackingResponse?> AddTrackingAsync(long orderId, AddTrackingRequest request);

        /// <summary>
        /// Cập nhật tracking
        /// </summary>
        Task<bool> UpdateTrackingAsync(long orderId, long trackingId, AddTrackingRequest request);

        /// <summary>
        /// Xóa tracking
        /// </summary>
        Task<bool> DeleteTrackingAsync(long orderId, long trackingId);

        /// <summary>
        /// Lấy thống kê orders
        /// </summary>
        Task<OrderStats?> GetOrderStatsAsync(DateTime? from = null, DateTime? to = null);
    }

    /// <summary>
    /// DTO cho Order Filter Request
    /// </summary>
    public class OrderFilterRequest
    {
        public OrderStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public long? UserId { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }
        public string? SortBy { get; set; } = "createdAt";
        public string? SortDir { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
