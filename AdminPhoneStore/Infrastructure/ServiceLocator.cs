using Microsoft.Extensions.DependencyInjection;

namespace AdminPhoneStore.Infrastructure
{
    /// <summary>
    /// Service Locator pattern để resolve services từ DI container
    /// Sử dụng trong XAML và code-behind khi cần
    /// </summary>
    public class ServiceLocator
    {
        private static ServiceProvider? _serviceProvider;

        public static void SetServiceProvider(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetService<T>() where T : class
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceProvider chưa được khởi tạo. Gọi SetServiceProvider trước.");

            return _serviceProvider.GetRequiredService<T>();
        }

        public static object? GetService(Type serviceType)
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceProvider chưa được khởi tạo. Gọi SetServiceProvider trước.");

            return _serviceProvider.GetService(serviceType);
        }
    }
}
