using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CatFactsApp.Services
{
    public class CatFactApplicationService : ICatFactApplicationService
    {
        private readonly ICatFactClient _catClient;
        private readonly IFileService _fileService;

        public CatFactApplicationService(ICatFactClient catClient, IFileService fileService)
        {
            _catClient = catClient;
            _fileService = fileService;
        }

        public async Task<FetchResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await _catClient.GetFactAsync(cancellationToken);
                stopwatch.Stop();

                if (response != null)
                {
                    Console.WriteLine("✓ Fact received");

                    await _fileService.SaveFactToFileAsync(response.Fact, cancellationToken);

                    Console.WriteLine("✓ Fact saved");
                    Console.WriteLine();
                    Console.WriteLine($"Fact length: {response.Length} characters");
                    Console.WriteLine($"Request duration: {stopwatch.ElapsedMilliseconds} ms");

                    return new FetchResult(true);
                }
                else
                {
                    Console.WriteLine("Failed to retrieve the fact (the response was empty).");
                    return new FetchResult(false, "Response was empty.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new FetchResult(false, ex.Message);
            }
        }
    }
}