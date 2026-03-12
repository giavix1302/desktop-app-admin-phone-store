using System.Text.Json.Serialization;

namespace AdminPhoneStore.Models
{
    /// <summary>
    /// Model cho Product theo API structure
    /// </summary>
    public class Product
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("discountPercent")]
        public decimal? DiscountPercent { get; set; }

        [JsonPropertyName("stockQuantity")]
        public int StockQuantity { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("categoryId")]
        public long? CategoryId { get; set; }

        [JsonPropertyName("categoryName")]
        public string? CategoryName { get; set; }

        [JsonPropertyName("brandId")]
        public long? BrandId { get; set; }

        [JsonPropertyName("brandName")]
        public string? BrandName { get; set; }

        [JsonPropertyName("colors")]
        public List<ProductColor> Colors { get; set; } = new();

        [JsonPropertyName("specifications")]
        public List<ProductSpecification> Specifications { get; set; } = new();

        [JsonPropertyName("images")]
        public List<ProductImageDto> Images { get; set; } = new();

        [JsonPropertyName("averageRating")]
        public double AverageRating { get; set; }

        [JsonPropertyName("totalReviews")]
        public int TotalReviews { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Giá thực tế sau giảm giá (tính ở frontend)
        /// </summary>
        public decimal FinalPrice => DiscountPercent.HasValue && DiscountPercent.Value > 0
            ? Price * (1 - DiscountPercent.Value / 100)
            : Price;
    }

    /// <summary>
    /// Model cho Product Color
    /// </summary>
    public class ProductColor
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("colorName")]
        public string ColorName { get; set; } = string.Empty;

        [JsonPropertyName("hexCode")]
        public string? HexCode { get; set; }
    }

    /// <summary>
    /// Model cho Product Specification
    /// </summary>
    public class ProductSpecification
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("specName")]
        public string SpecName { get; set; } = string.Empty;

        [JsonPropertyName("specValue")]
        public string SpecValue { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model cho Product Image (response DTO từ API)
    /// </summary>
    public class ProductImageDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("productId")]
        public long ProductId { get; set; }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("altText")]
        public string? AltText { get; set; }

        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; set; }
    }

    /// <summary>
    /// DTO cho Product List Response (paged) theo API
    /// </summary>
    public class ProductPagedResponse
    {
        [JsonPropertyName("items")]
        public List<Product> Items { get; set; } = new();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage { get; set; }

        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage { get; set; }
    }

    /// <summary>
    /// DTO cho Create Product Request
    /// </summary>
    public class CreateProductRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("discountPercent")]
        public decimal? DiscountPercent { get; set; }

        [JsonPropertyName("stockQuantity")]
        public int StockQuantity { get; set; }

        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("brandId")]
        public long BrandId { get; set; }

        [JsonPropertyName("colorIds")]
        public List<long> ColorIds { get; set; } = new();

        [JsonPropertyName("specifications")]
        public List<SpecificationRequest>? Specifications { get; set; }
    }

    /// <summary>
    /// DTO cho Update Product Request
    /// </summary>
    public class UpdateProductRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("discountPercent")]
        public decimal? DiscountPercent { get; set; }

        [JsonPropertyName("stockQuantity")]
        public int StockQuantity { get; set; }

        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("brandId")]
        public long BrandId { get; set; }

        [JsonPropertyName("colorIds")]
        public List<long>? ColorIds { get; set; }

        [JsonPropertyName("specifications")]
        public List<SpecificationRequest>? Specifications { get; set; }
    }

    /// <summary>
    /// DTO cho Specification Request
    /// </summary>
    public class SpecificationRequest
    {
        [JsonPropertyName("specName")]
        public string SpecName { get; set; } = string.Empty;

        [JsonPropertyName("specValue")]
        public string SpecValue { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho Product Filter Request
    /// </summary>
    public class ProductFilterRequest
    {
        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } = "createdAt";
        public string? SortDir { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
    }
}
