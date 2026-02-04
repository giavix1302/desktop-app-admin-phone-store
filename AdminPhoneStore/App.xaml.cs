using AdminPhoneStore.Infrastructure;
using AdminPhoneStore.Services.Auth;
using AdminPhoneStore.Services.Api;
using AdminPhoneStore.Services.UI;
using AdminPhoneStore.Services.Business;
using AdminPhoneStore.Services.Infrastructure;
using AdminPhoneStore.ViewModels.Auth;
using AdminPhoneStore.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;

namespace AdminPhoneStore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            try
            {
                await InitializeAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Startup failed: {ex.Message}");
                MessageBox.Show(
                    $"Lỗi khởi động ứng dụng:\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown();
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                // Setup Dependency Injection
                var services = new ServiceCollection();
                ConfigureServices(services);

                _serviceProvider = services.BuildServiceProvider();
                ServiceLocator.SetServiceProvider(_serviceProvider);

                // Setup Global Exception Handler
                var loggerService = _serviceProvider.GetService<ILoggerService>();
                var dialogService = _serviceProvider.GetService<IDialogService>();
                var globalExceptionHandler = new GlobalExceptionHandler(dialogService, loggerService);
                globalExceptionHandler.Setup();

                // Load tokens from storage (restore session)
                var authService = _serviceProvider.GetService<IAuthenticationService>();
                bool isAuthenticated = false;

                if (authService != null)
                {
                    try
                    {
                        isAuthenticated = await authService.LoadTokensFromStorageAsync();
                    }
                    catch (Exception ex)
                    {
                        loggerService?.LogError("Failed to load tokens from storage", ex);
                        isAuthenticated = false;
                    }
                }

                // Hiển thị LoginWindow hoặc MainWindow dựa vào authentication state
                // Phải invoke trên UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        if (isAuthenticated && authService?.IsAuthenticated == true)
                        {
                            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                            Application.Current.MainWindow = mainWindow; // ⭐ BẮT BUỘC
                            mainWindow.Show();
                        }
                        else
                        {
                            var loginWindow = _serviceProvider.GetRequiredService<Windows.Auth.LoginWindow>();
                            Application.Current.MainWindow = loginWindow; // ⭐ BẮT BUỘC
                            loginWindow.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error showing window: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                        loggerService?.LogError("Failed to show window", ex);
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeAsync exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw để ContinueWith catch
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register Infrastructure Services (singleton)
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<ITokenStorageService, TokenStorageService>();

            // Register API Configuration (singleton)
            services.AddSingleton<IApiConfiguration, ApiConfiguration>();

            // Register HttpClient (singleton để tái sử dụng connection)
            services.AddSingleton<HttpClient>(sp =>
            {
                var httpClient = new HttpClient();
                return httpClient;
            });

            // Register Authentication Service (singleton - dùng HttpClient trực tiếp, không phụ thuộc ApiClientService)
            services.AddSingleton<IAuthenticationService>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                var apiConfig = sp.GetRequiredService<IApiConfiguration>();
                var tokenStorage = sp.GetRequiredService<ITokenStorageService>();
                var logger = sp.GetService<ILoggerService>();
                return new AuthenticationService(httpClient, apiConfig, tokenStorage, logger);
            });

            // Register API Client Service (singleton - có thể dùng IAuthenticationService để refresh token)
            services.AddSingleton<IApiClientService>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                var apiConfig = sp.GetRequiredService<IApiConfiguration>();
                var logger = sp.GetService<ILoggerService>();
                var authService = sp.GetService<IAuthenticationService>();
                return new ApiClientService(httpClient, apiConfig, logger, authService);
            });

            // Register UI Services (singleton)
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();

            // Register Business Services (transient)
            services.AddTransient<IProductService, ProductService>();

            // Register ViewModels
            services.AddTransient<ShellViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<LoginViewModel>();

            // Register Views
            services.AddTransient<Views.Pages.DashboardView>();
            services.AddTransient<Views.Pages.ProductView>();
            services.AddTransient<Views.Auth.LoginView>();
            services.AddTransient<Views.Dialogs.ToastView>();
            services.AddTransient<Views.Dialogs.ConfirmDialogView>();

            // Register Windows
            services.AddTransient<MainWindow>();
            services.AddTransient<Windows.Auth.LoginWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
