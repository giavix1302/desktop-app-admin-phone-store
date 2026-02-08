using AdminPhoneStore.Models;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Service để quản lý Brand
    /// </summary>
    public interface IBrandService
    {
        Task<List<Brand>> GetAllBrandsAsync();
        Task<Brand?> GetBrandByIdAsync(long id);
        Task<Brand?> CreateBrandAsync(CreateBrandRequest request);
        Task<Brand?> UpdateBrandAsync(long id, UpdateBrandRequest request);
        Task<bool> DeleteBrandAsync(long id);
    }
}
