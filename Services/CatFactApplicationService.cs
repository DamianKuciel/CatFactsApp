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
                Console.WriteLine("Fetching a random cat fact...");
                var response = await _catClient.GetFactAsync(cancellationToken);

                if (response != null)
                {
                    await _fileService.SaveFactToFileAsync(response.Fact, cancellationToken);
                    Console.WriteLine($"Cat fact saved to file: {response.Fact}");
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