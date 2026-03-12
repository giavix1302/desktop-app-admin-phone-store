using AdminPhoneStore.Models;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Service để quản lý Review (Admin)
    /// </summary>
    public interface IReviewService
    {
        /// <summary>
        /// Lấy danh sách reviews với filter, sort và pagination
        /// </summary>
        Task<PagedResponse<Review>> GetReviewsAsync(ReviewFilterRequest? filter = null);

        /// <summary>
        /// Lấy chi tiết review theo ID
        /// </summary>
        Task<ReviewDetail?> GetReviewByIdAsync(long reviewId);

        /// <summary>
        /// Xóa review (Admin)
        /// </summary>
        Task<bool> DeleteReviewAsync(long reviewId);

        /// <summary>
        /// Lấy thống kê reviews
        /// </summary>
        Task<ReviewStats?> GetReviewStatsAsync(DateTime? from = null, DateTime? to = null);
    }

    /// <summary>
    /// DTO cho Review Filter Request
    /// </summary>
    public class ReviewFilterRequest
    {
        public long? ProductId { get; set; }
        public long? UserId { get; set; }
        public int? Rating { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? SortBy { get; set; } = "createdAt";
        public string? SortDir { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
