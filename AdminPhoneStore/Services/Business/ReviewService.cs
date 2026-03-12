using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Implementation của IReviewService
    /// </summary>
    public class ReviewService : IReviewService
    {
        private readonly IApiClientService _apiClient;

        public ReviewService(IApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<PagedResponse<Review>> GetReviewsAsync(ReviewFilterRequest? filter = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (filter != null)
            {
                if (filter.ProductId.HasValue)
                    queryParams["productId"] = filter.ProductId.Value.ToString();
                if (filter.UserId.HasValue)
                    queryParams["userId"] = filter.UserId.Value.ToString();
                if (filter.Rating.HasValue)
                    queryParams["rating"] = filter.Rating.Value.ToString();
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

            return await _apiClient.GetAsync<PagedResponse<Review>>("/api/admin/reviews", queryParams)
                   ?? new PagedResponse<Review>();
        }

        public async Task<ReviewDetail?> GetReviewByIdAsync(long reviewId)
        {
            return await _apiClient.GetAsync<ReviewDetail>($"/api/admin/reviews/{reviewId}");
        }

        public async Task<bool> DeleteReviewAsync(long reviewId)
        {
            return await _apiClient.DeleteAsync($"/api/admin/reviews/{reviewId}");
        }

        public async Task<ReviewStats?> GetReviewStatsAsync(DateTime? from = null, DateTime? to = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (from.HasValue)
                queryParams["from"] = from.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
            if (to.HasValue)
                queryParams["to"] = to.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");

            return await _apiClient.GetAsync<ReviewStats>("/api/admin/reviews/stats", queryParams);
        }
    }
}
