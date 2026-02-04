using AdminPhoneStore.Services.Api;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AdminPhoneStore.Services.Api
{
    /// <summary>
    /// Implementation của IApiConfiguration đọc từ appsettings.json
    /// </summary>
    public class ApiConfiguration : IApiConfiguration
    {
        private readonly IConfiguration _configuration;

        public string BaseUrl { get; }
        public int TimeoutSeconds { get; }

        public ApiConfiguration()
        {
            // Load appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            // Đọc từ appsettings.json
            BaseUrl = _configuration["ApiSettings:BaseUrl"] 
                ?? throw new InvalidOperationException("ApiSettings:BaseUrl không được cấu hình trong appsettings.json");
            
            TimeoutSeconds = int.Parse(_configuration["ApiSettings:TimeoutSeconds"] ?? "30");
        }

        // Constructor để set custom URL (cho testing)
        public ApiConfiguration(string baseUrl, int timeoutSeconds = 30)
        {
            BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            TimeoutSeconds = timeoutSeconds;
            _configuration = new ConfigurationBuilder().Build();
        }
    }
}
