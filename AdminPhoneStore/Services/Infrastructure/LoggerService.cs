using AdminPhoneStore.Services.Infrastructure;
using System.IO;

namespace AdminPhoneStore.Services.Infrastructure
{
    /// <summary>
    /// Simple file logger implementation
    /// TODO: Có thể thay thế bằng Serilog sau này
    /// </summary>
    public class LoggerService : ILoggerService
    {
        private readonly string _logFilePath;
        private readonly object _lockObject = new object();

        public LoggerService()
        {
            var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDirectory);
            _logFilePath = Path.Combine(logDirectory, $"app_{DateTime.Now:yyyyMMdd}.log");
        }

        public void LogInformation(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogWarning(string message)
        {
            WriteLog("WARN", message);
        }

        public void LogError(string message, Exception? exception = null)
        {
            var errorMessage = exception != null 
                ? $"{message}\nException: {exception.Message}\nStack Trace: {exception.StackTrace}" 
                : message;
            WriteLog("ERROR", errorMessage);
        }

        public void LogDebug(string message)
        {
            WriteLog("DEBUG", message);
        }

        private void WriteLog(string level, string message)
        {
            lock (_lockObject)
            {
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}\n";
                File.AppendAllText(_logFilePath, logEntry);
            }
        }
    }
}
