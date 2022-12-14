using ExchangeRates.Client.Interfaces;
using ExchangeRates.Client.Options;
using ExchangeRates.Client.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ExchangeRates.Client
{
    internal sealed class Program
    {
        [STAThread]
        public static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    configuration
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    var configurationRoot = hostingContext.Configuration;
                    services.Configure<ApiOptions>(configurationRoot.GetSection(ApiOptions.SectionName));
                    services.AddHttpClient();
                    services.AddSingleton<App>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<IApiService, ApiService>();
                })
                .Build();
            // получаем сервис - объект класса App
            var app = host.Services.GetService<App>();
            // запускаем приложения
            app?.Run();
        }
    }
}
