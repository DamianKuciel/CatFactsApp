namespace CatFactsApp.Services
{
    public interface ICatFactApplicationService
    {
        Task<FetchResult> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}