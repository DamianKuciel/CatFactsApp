using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using CatFactsApp.Configuration;
using CatFactsApp.Services;
using System.Linq.Expressions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory);
        config.AddJsonFile("appsettings.json", optional:false, reloadOnChange: true);
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
         .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

         services.AddTransient<IFileService, FileService>();
     })
    .Build();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var catClient = services.GetRequiredService<ICatFactClient>();
    var fileService = services.GetRequiredService<IFileService>();

    Console.WriteLine("Fetching a random cat fact...");
    var response = await catClient.GetFactAsync();

    if (response != null)
    {
        await fileService.SaveFactToFileAsync(response.Fact);
        Console.WriteLine($"Cat fact saved to file: {response.Fact}");
    }
    else
    {
        Console.WriteLine("Failed to retrieve the fact (the response was empty).");

    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}


