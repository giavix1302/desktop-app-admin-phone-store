using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AdminPhoneStore.Services.Interfaces;
using AdminPhoneStore.Services.Implementations;
using AdminPhoneStore.ViewModels;
using AdminPhoneStore.Infrastructure;

namespace AdminPhoneStore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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
            if (authService != null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await authService.LoadTokensFromStorageAsync();
                    }
                    catch (Exception ex)
                    {
                        loggerService?.LogError("Failed to load tokens from storage", ex);
                    }
                });
            }

            // Tạo MainWindow với DI
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register Logger Service (singleton)
            services.AddSingleton<ILoggerService, LoggerService>();

            // Register Configuration
            services.AddSingleton<IApiConfiguration, ApiConfiguration>();

            // Register Token Storage Service (singleton)
            services.AddSingleton<ITokenStorageService, TokenStorageService>();

            // Register HttpClient (singleton để tái sử dụng connection)
            services.AddSingleton<HttpClient>(sp =>
            {
                var httpClient = new HttpClient();
                return httpClient;
            });

            // Register Authentication Service (singleton - must be before ApiClientService)
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            // Register API Client Service (singleton - needs IAuthenticationService)
            // Use factory to resolve IAuthenticationService
            services.AddSingleton<IApiClientService>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                var apiConfig = sp.GetRequiredService<IApiConfiguration>();
                var logger = sp.GetService<ILoggerService>();
                var authService = sp.GetService<IAuthenticationService>();
                return new ApiClientService(httpClient, apiConfig, logger, authService);
            });

            // Register Business Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddTransient<IProductService, ProductService>();

            // Register ViewModels
            services.AddTransient<ShellViewModel>();
            services.AddTransient<DashboardViewModel>();

            // Register Views
            services.AddTransient<Views.DashboardView>();
            services.AddTransient<Views.ProductView>();

            // Register MainWindow
            services.AddTransient<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
