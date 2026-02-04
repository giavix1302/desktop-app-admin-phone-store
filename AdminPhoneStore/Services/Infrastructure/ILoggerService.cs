namespace AdminPhoneStore.Services.Infrastructure
{
    /// <summary>
    /// Service để logging (abstraction layer, có thể thay đổi implementation)
    /// </summary>
    public interface ILoggerService
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? exception = null);
        void LogDebug(string message);
    }
}
