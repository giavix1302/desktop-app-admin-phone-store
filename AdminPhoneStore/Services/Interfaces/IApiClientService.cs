namespace AdminPhoneStore.Services.Interfaces
{
    /// <summary>
    /// Service để gọi API backend
    /// </summary>
    public interface IApiClientService
    {
        /// <summary>
        /// GET request
        /// </summary>
        Task<T?> GetAsync<T>(string endpoint) where T : class;

        /// <summary>
        /// GET request với query parameters
        /// </summary>
        Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null) where T : class;

        /// <summary>
        /// POST request
        /// </summary>
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data) 
            where TRequest : class 
            where TResponse : class;

        /// <summary>
        /// PUT request
        /// </summary>
        Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data) 
            where TRequest : class 
            where TResponse : class;

        /// <summary>
        /// DELETE request
        /// </summary>
        Task<bool> DeleteAsync(string endpoint);

        /// <summary>
        /// Set authentication token (nếu API cần)
        /// </summary>
        void SetAuthToken(string? token);
    }
}
