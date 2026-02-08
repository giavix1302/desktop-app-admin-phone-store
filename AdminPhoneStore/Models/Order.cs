using System.Text.Json.Serialization;

namespace AdminPhoneStore.Models
{
    /// <summary>
    /// Enum cho Order Status
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        PENDING,
        PROCESSING,
        SHIPPED,
        DELIVERED,
        CANCELLED
    }

    /// <summary>
    /// Enum cho Payment Method
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentMethod
    {
        COD,
        MOMO,
        BANK
    }

    /// <summary>
    /// Enum cho Payment Status
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentStatus
    {
        PENDING,
        PAID,
        FAILED,
        REFUNDED
    }

    /// <summary>
    /// Model cho Order (list view - simplified)
    /// </summary>
    public class Order
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("orderNumber")]
        public string OrderNumber { get; set; } = string.Empty;
        
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        
        [JsonPropertyName("userEmail")]
        public string? UserEmail { get; set; }
        
        [JsonPropertyName("totalAmount")]
        public decimal TotalAmount { get; set; }
        
        [JsonPropertyName("status")]
        public OrderStatus Status { get; set; }
        
        [JsonPropertyName("paymentMethod")]
        public PaymentMethod PaymentMethod { get; set; }
        
        [JsonPropertyName("paymentStatus")]
        public PaymentStatus PaymentStatus { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        
        [JsonPropertyName("itemCount")]
        public int ItemCount { get; set; }
    }

    /// <summary>
    /// Model cho Order Detail (full information)
    /// </summary>
    public class OrderDetail
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? UserName { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string? ShippingAddress { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public List<OrderTracking> Trackings { get; set; } = new();
    }

    /// <summary>
    /// Model cho Order Item
    /// </summary>
    public class OrderItem
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSlug { get; set; } = string.Empty;
        public long? ColorId { get; set; }
        public string? ColorName { get; set; }
        public string? ColorHexCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// Model cho Order Tracking
    /// </summary>
    public class OrderTracking
    {
        public long Id { get; set; }
        public OrderStatus Status { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public string? TrackingNumber { get; set; }
        public string? ShippingPattern { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Model cho Order Stats
    /// </summary>
    public class OrderStats
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public Dictionary<string, int> CountByStatus { get; set; } = new();
    }

    /// <summary>
    /// Model cho Paged Response
    /// </summary>
    public class PagedResponse<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = new();
        
        [JsonPropertyName("page")]
        public int Page { get; set; }
        
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
        
        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }
        
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// DTO cho Update Order Status Request
    /// </summary>
    public class UpdateOrderStatusRequest
    {
        public OrderStatus Status { get; set; }
    }

    /// <summary>
    /// DTO cho Update Order Payment Request
    /// </summary>
    public class UpdateOrderPaymentRequest
    {
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
    }

    /// <summary>
    /// DTO cho Add/Update Tracking Request
    /// </summary>
    public class AddTrackingRequest
    {
        public OrderStatus Status { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public string? TrackingNumber { get; set; }
        public string? ShippingPattern { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
    }

    /// <summary>
    /// DTO cho Add Tracking Response
    /// </summary>
    public class AddTrackingResponse
    {
        public long Id { get; set; }
    }
}
