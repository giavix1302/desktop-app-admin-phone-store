using AdminPhoneStore.Models;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Service để quản lý Color
    /// </summary>
    public interface IColorService
    {
        Task<List<Color>> GetAllColorsAsync();
        Task<Color?> GetColorByIdAsync(long id);
    }
}
