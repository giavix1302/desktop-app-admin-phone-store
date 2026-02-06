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
    }
}
