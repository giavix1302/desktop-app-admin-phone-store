using AdminPhoneStore.Models;

namespace AdminPhoneStore.Services.Business
{
    /// <summary>
    /// Service để quản lý User (Admin)
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Lấy danh sách users với filter, search, sort và pagination
        /// </summary>
        Task<PagedResponse<User>> GetUsersAsync(UserFilterRequest? filter = null);

        /// <summary>
        /// Lấy chi tiết user theo ID
        /// </summary>
        Task<UserDetail?> GetUserByIdAsync(long userId);
    }

    /// <summary>
    /// DTO cho User Filter Request
    /// </summary>
    public class UserFilterRequest
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? SortBy { get; set; } = "createdAt";
        public string? SortDir { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
