using System.Windows;
using System.Windows.Threading;
using AdminPhoneStore.Services.Interfaces;

namespace AdminPhoneStore.Infrastructure
{
    /// <summary>
    /// Global exception handler để catch tất cả unhandled exceptions
    /// </summary>
    public class GlobalExceptionHandler
    {
        private readonly IDialogService? _dialogService;
        private readonly ILoggerService? _loggerService;

        public GlobalExceptionHandler(IDialogService? dialogService = null, ILoggerService? loggerService = null)
        {
            _dialogService = dialogService;
            _loggerService = loggerService;
        }

        public void Setup()
        {
            // Handle UI thread exceptions
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;

            // Handle non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            // Handle Task exceptions
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception, "UI Thread Exception");
            e.Handled = true; // Mark as handled để app không crash
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                HandleException(ex, "Unhandled Exception");
            }
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception, "Task Exception");
            e.SetObserved(); // Mark as observed
        }

        private void HandleException(Exception exception, string source)
        {
            // Log exception
            _loggerService?.LogError($"[{source}] {exception.Message}", exception);

            // Show error dialog to user
            _dialogService?.ShowError(
                $"Đã xảy ra lỗi không mong muốn.\n\nChi tiết: {exception.Message}\n\nVui lòng kiểm tra log file để biết thêm thông tin.",
                "Lỗi Ứng Dụng");
        }
    }
}
