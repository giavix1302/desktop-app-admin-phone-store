namespace AdminPhoneStore.Models
{
    /// <summary>
    /// Model cho Brand theo API structure
    /// </summary>
    public class Brand
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO cho Create/Update Brand Request
    /// </summary>
    public class CreateBrandRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// DTO cho Update Brand Request
    /// </summary>
    public class UpdateBrandRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
