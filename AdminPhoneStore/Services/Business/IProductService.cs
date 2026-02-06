using AdminPhoneStore.Models;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Service để quản lý business logic liên quan đến Product
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Lấy tất cả products
        /// </summary>
        Task<List<Product>> GetAllProductsAsync();

        /// <summary>
        /// Lấy product theo ID
        /// </summary>
        Task<Product?> GetProductByIdAsync(long id);

        /// <summary>
        /// Tạo product mới
        /// </summary>
        Task<Product?> CreateProductAsync(CreateProductRequest request);

        /// <summary>
        /// Cập nhật product
        /// </summary>
        Task<Product?> UpdateProductAsync(long id, UpdateProductRequest request);

        /// <summary>
        /// Xóa product
        /// </summary>
        Task<bool> DeleteProductAsync(long id);
    }
}
