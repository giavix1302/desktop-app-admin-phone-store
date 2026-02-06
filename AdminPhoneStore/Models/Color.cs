namespace AdminPhoneStore.Models
{
    /// <summary>
    /// Model cho Color theo API structure
    /// </summary>
    public class Color
    {
        public long Id { get; set; }
        public string ColorName { get; set; } = string.Empty;
        public string? HexCode { get; set; }
    }

    /// <summary>
    /// DTO cho Create/Update Color Request
    /// </summary>
    public class CreateColorRequest
    {
        public string ColorName { get; set; } = string.Empty;
        public string? HexCode { get; set; }
    }

    /// <summary>
    /// DTO cho Update Color Request
    /// </summary>
    public class UpdateColorRequest
    {
        public string ColorName { get; set; } = string.Empty;
        public string? HexCode { get; set; }
    }
}
