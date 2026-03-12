using AdminPhoneStore.Models;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Service để quản lý Category
    /// </summary>
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(long id);
        Task<Category?> CreateCategoryAsync(CreateCategoryRequest request);
        Task<Category?> UpdateCategoryAsync(long id, UpdateCategoryRequest request);
        Task<bool> DeleteCategoryAsync(long id);
        Task<Category?> UploadImageAsync(long id, string filePath);
        Task<bool> DeleteImageAsync(long id);
    }
}
