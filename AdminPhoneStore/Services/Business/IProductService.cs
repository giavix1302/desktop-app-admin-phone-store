using AdminPhoneStore.Models;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Service để quản lý business logic liên quan đến Product
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Lấy danh sách products (có filter, phân trang)
        /// </summary>
        Task<ProductPagedResponse> GetAllProductsAsync(ProductFilterRequest? filter = null);

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
        /// Xóa product (soft delete - set isActive = false)
        /// </summary>
        Task<bool> DeleteProductAsync(long id);
    }
}
