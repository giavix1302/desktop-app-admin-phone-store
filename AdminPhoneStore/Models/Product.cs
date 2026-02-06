namespace AdminPhoneStore.Models
{
    /// <summary>
    /// Model cho Product theo API structure
    /// </summary>
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public long? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public long? BrandId { get; set; }
        public string? BrandName { get; set; }
        public List<ProductColor> Colors { get; set; } = new();
        public List<ProductSpecification> Specifications { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Model cho Product Color
    /// </summary>
    public class ProductColor
    {
        public long Id { get; set; }
        public string ColorName { get; set; } = string.Empty;
        public string? HexCode { get; set; }
    }

    /// <summary>
    /// Model cho Product Specification
    /// </summary>
    public class ProductSpecification
    {
        public long Id { get; set; }
        public string SpecName { get; set; } = string.Empty;
        public string SpecValue { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho Create/Update Product Request
    /// </summary>
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public long CategoryId { get; set; }
        public long BrandId { get; set; }
        public List<long> ColorIds { get; set; } = new();
        public List<SpecificationRequest>? Specifications { get; set; }
    }

    /// <summary>
    /// DTO cho Update Product Request
    /// </summary>
    public class UpdateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool? IsActive { get; set; }
        public long CategoryId { get; set; }
        public long BrandId { get; set; }
        public List<long>? ColorIds { get; set; }
        public List<SpecificationRequest>? Specifications { get; set; }
    }

    /// <summary>
    /// DTO cho Specification Request
    /// </summary>
    public class SpecificationRequest
    {
        public string SpecName { get; set; } = string.Empty;
        public string SpecValue { get; set; } = string.Empty;
    }
}
