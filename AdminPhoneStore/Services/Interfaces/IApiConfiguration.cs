namespace AdminPhoneStore.Services.Interfaces
{
    /// <summary>
    /// Configuration cho API endpoint
    /// </summary>
    public interface IApiConfiguration
    {
        /// <summary>
        /// Base URL của API backend
        /// </summary>
        string BaseUrl { get; }

        /// <summary>
        /// Timeout cho API requests (seconds)
        /// </summary>
        int TimeoutSeconds { get; }
    }
}
