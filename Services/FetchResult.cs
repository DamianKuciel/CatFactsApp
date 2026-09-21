namespace CatFactsApp.Services
{
    public record FetchResult(bool IsSuccess, string? ErrorMessage = null);
}