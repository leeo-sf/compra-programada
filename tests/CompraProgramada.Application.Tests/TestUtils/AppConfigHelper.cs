using CompraProgramada.Application.Config;
using Microsoft.Extensions.Configuration;

namespace CompraProgramada.Application.Tests.TestUtils;

public class AppConfigHelper
{
    public static AppConfig GetAppConfig()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        AppConfig appConfig = new();
        configuration.GetSection("ApplicationConfig").Bind(appConfig);

        return appConfig;
    }
}