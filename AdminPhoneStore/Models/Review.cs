using System.Text.Json.Serialization;

namespace AdminPhoneStore.Models
{
    /// <summary>
    /// Model cho Review item (list view)
    /// </summary>
    public class Review
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("userEmail")]
        public string? UserEmail { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("productId")]
        public long ProductId { get; set; }

        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("orderItemId")]
        public long? OrderItemId { get; set; }

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Model cho Review Detail (full information)
    /// </summary>
    public class ReviewDetail
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("userEmail")]
        public string? UserEmail { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("userAvatarUrl")]
        public string? UserAvatarUrl { get; set; }

        [JsonPropertyName("productId")]
        public long ProductId { get; set; }

        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("productSlug")]
        public string? ProductSlug { get; set; }

        [JsonPropertyName("orderItemId")]
        public long? OrderItemId { get; set; }

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Model cho Review Stats
    /// </summary>
    public class ReviewStats
    {
        [JsonPropertyName("totalReviews")]
        public int TotalReviews { get; set; }

        [JsonPropertyName("averageRating")]
        public double AverageRating { get; set; }

        [JsonPropertyName("ratingDistribution")]
        public Dictionary<string, int> RatingDistribution { get; set; } = new();

        [JsonPropertyName("percentageDistribution")]
        public Dictionary<string, double> PercentageDistribution { get; set; } = new();

        [JsonPropertyName("reviewsToday")]
        public int ReviewsToday { get; set; }

        [JsonPropertyName("reviewsThisWeek")]
        public int ReviewsThisWeek { get; set; }

        [JsonPropertyName("reviewsThisMonth")]
        public int ReviewsThisMonth { get; set; }
    }
}
