using AdminPhoneStore.Models;

namespace AdminPhoneStore.Services.Interfaces
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
        Task<Product?> GetProductByIdAsync(int id);

        /// <summary>
        /// Tạo product mới
        /// </summary>
        Task<Product?> CreateProductAsync(Product product);

        /// <summary>
        /// Cập nhật product
        /// </summary>
        Task<Product?> UpdateProductAsync(int id, Product product);

        /// <summary>
        /// Xóa product
        /// </summary>
        Task<bool> DeleteProductAsync(int id);

        /// <summary>
        /// Tìm kiếm products
        /// </summary>
        Task<List<Product>> SearchProductsAsync(string searchTerm);
    }
}
