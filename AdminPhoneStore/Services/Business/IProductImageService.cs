using AdminPhoneStore.Models;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Service để quản lý Product Images
    /// </summary>
    public interface IProductImageService
    {
        /// <summary>
        /// Lấy danh sách tất cả images của một product
        /// </summary>
        Task<List<ProductImage>> GetProductImagesAsync(long productId);

        /// <summary>
        /// Upload image mới cho product
        /// </summary>
        Task<ProductImage> UploadProductImageAsync(long productId, string imageFilePath, string? altText = null, bool isPrimary = false);

        /// <summary>
        /// Update image (metadata hoặc replace file)
        /// </summary>
        Task<ProductImage> UpdateProductImageAsync(long productId, long imageId, string? imageFilePath = null, string? altText = null, bool? isPrimary = null);

        /// <summary>
        /// Xóa image
        /// </summary>
        Task<bool> DeleteProductImageAsync(long productId, long imageId);
    }
}
