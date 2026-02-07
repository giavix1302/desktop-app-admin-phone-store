namespace AdminPhoneStore.Models
{
    /// <summary>
    /// Model cho Product Image theo API structure
    /// </summary>
    public class ProductImage
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public bool IsPrimary { get; set; }
    }

    /// <summary>
    /// DTO cho Update Product Image Request
    /// </summary>
    public class UpdateProductImageRequest
    {
        public string? AltText { get; set; }
        public bool? IsPrimary { get; set; }
    }
}
