using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CatFactsApp.Configuration;
using CatFactsApp.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.Configure<AppOptions>(configuration.GetSection("AppOptions"));

        var apiUrl = configuration["AppOptions:ApiUrl"] ?? "https://catfact.ninja/fact";

        services.AddHttpClient<ICatFactClient, CatFactClient>(client =>
        {
            client.BaseAddress = new Uri(apiUrl);
        })
        .AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 3;
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddTransient<IFileService, FileService>();
        services.AddTransient<ICatFactApplicationService, CatFactApplicationService>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var provider = scope.ServiceProvider;

var appService = provider.GetRequiredService<ICatFactApplicationService>();

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("\nAnulowanie operacji...");
    e.Cancel = true;
    cts.Cancel();
};

var result = await appService.ExecuteAsync(cts.Token);

return result.IsSuccess ? 0 : 1;